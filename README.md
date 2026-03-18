# מערכת מכירות פומביות בזמן אמת

## מבנה הפרויקט
```
AuctionSystem/
├── AuctionSystem.API/         ← Backend (.NET 8)
│   ├── Models/                ← ישויות DB (Auction, Bid)
│   ├── DTOs/                  ← אובייקטי העברת נתונים
│   ├── Interfaces/            ← הגדרות חוזים לServices
│   ├── Services/              ← לוגיקה עסקית
│   ├── Data/                  ← DbContext
│   ├── Hubs/                  ← SignalR Hub
│   ├── Routes/                ← Endpoints מחולקים לפי נושא
│   └── Program.cs             ← רישום שירותים ו-Middleware
│
└── auction-client/            ← Frontend (Angular 17)
    └── src/app/
        ├── models/            ← Interfaces של נתונים
        ├── services/          ← HTTP + SignalR
        └── components/        ← auction-list, auction-detail
```

---

## החלטות ארכיטקטורה מרכזיות

- **Minimal API** במקום Controllers – קוד קצר וקריא יותר, כל endpoint מוגדר ישירות ב-`Program.cs` דרך קבצי Routes
- **הפרדת שכבות** – Controller רזה, לוגיקה עסקית ב-Service, גישה לנתונים דרך DbContext בלבד
- **Dependency Injection** – כל Service מוזרק דרך Interface, מה שמאפשר החלפה ובדיקה קלה
- **Standalone Components** ב-Angular – כל קומפוננטה עצמאית עם imports משלה, ללא NgModules
- **Signal אחד גלובלי** ל-SignalR ב-Angular – חיבור אחד לכל האפליקציה, לא נסגר בין ניווטים

---

## אופן שמירת הנתונים

- **SQL Server** עם **Entity Framework Core**
- שתי טבלאות: `Auctions`, `Bids`
- `Auction` מכיל `RowVersion` לטיפול ב-Concurrency
- Seed Data – שתי מכירות לדוגמה נוצרות אוטומטית במיגרציה
```
Auctions
├── Id, Title, Description
├── StartingPrice, CurrentPrice
├── StartTime, EndTime
├── Status (Active / Closed)
└── RowVersion (לConcurrency)

Bids
├── Id, AuctionId
├── BidderName, Amount
└── PlacedAt
```

---

## מימוש עדכונים בזמן אמת

המערכת משתמשת ב-**SignalR** לעדכונים דו-כיווניים בין השרת לכל הלקוחות המחוברים.

**זרימת עדכון:**
1. משתמש מגיש הצעה → HTTP POST לשרת
2. השרת מעדכן את ה-DB
3. השרת שולח event דרך SignalR לשתי קבוצות:
   - `auction_{id}` – כל המשתמשים שנמצאים במסך המכירה הספציפית
   - `auction_list` – כל המשתמשים שנמצאים ברשימת המכירות
4. ה-Frontend מקבל את ה-event ומעדכן את ה-UI ישירות ללא רענון

**Events:**
- `BidPlaced` – הצעה חדשה התקבלה, כולל המחיר החדש ושם המציע
- `AuctionClosed` – מכירה נסגרה, כולל המחיר הסופי

---

## אופן פעולת תהליך הרקע

`AuctionExpiryService` יורש מ-`BackgroundService` ורץ ברקע ללא תלות בבקשות HTTP.

**אופן פעולה:**
1. כל **30 שניות** – סורק את ה-DB לאיתור מכירות שהסתיימו
2. מכירות שה-`EndTime` שלהן עבר וסטטוסן `Active` – מועברות ל-`Closed`
3. השינוי נשמר ב-DB
4. כל המשתמשים המחוברים מקבלים event `AuctionClosed` דרך SignalR

**הערה טכנית:** מכיוון ש-`BackgroundService` הוא Singleton, נעשה שימוש ב-`IServiceScopeFactory` ליצירת Scope ידני לכל ריצה – כדי לאפשר שימוש ב-`DbContext` שהוא Scoped.

---

## טיפול ב-Concurrency

**הבעיה:** מספר משתמשים יכולים להגיש הצעות באותו רגע, מה שעלול לגרום לכך שהצעה נמוכה תדרוס הצעה גבוהה יותר.

**הפתרון – Optimistic Locking עם RowVersion:**

- לכל רשומת `Auction` יש עמודת `RowVersion` שמתעדכנת אוטומטית בכל שמירה
- כש-EF Core מעדכן את המחיר, הוא מוסיף לשאילתה תנאי `WHERE RowVersion = @currentVersion`
- אם משתמש אחר עדכן קודם – ה-`RowVersion` השתנה, השאילתה לא תמצא את השורה, ו-EF Core יזרוק `DbUpdateConcurrencyException`
- במקרה כזה – השרת מחזיר שגיאה ללקוח עם המחיר העדכני, והמשתמש מתבקש לנסות שוב
```csharp
try {
    await _context.SaveChangesAsync();
}
catch (DbUpdateConcurrencyException) {
    await _context.Entry(auction).ReloadAsync();
    throw new InvalidOperationException(
        $"המחיר התעדכן ל-{auction.CurrentPrice}, נסה שוב");
}
```

**יתרון על Pessimistic Locking:** אין נעילת שורות ב-DB, ביצועים טובים יותר במקרה הרגיל שבו אין התנגשות.