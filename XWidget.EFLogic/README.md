XWidget.EFLogic
=====
[![NuGet](https://img.shields.io/nuget/v/XWidget.EFLogic.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.EFLogic/)
[![NuGet](https://img.shields.io/nuget/dt/XWidget.EFLogic.svg?style=flat-square)](https://www.nuget.org/packages/XWidget.EFLogic/)
[![GitHub](https://img.shields.io/github/license/XuPeiYao/XWidget.svg?style=flat-square)](https://github.com/XuPeiYao/XWidget/blob/master/LICENSE)

針對EntityFrameworkCore的邏輯層存取架構

## 快速上手
### 1.建立LogicManager類型
```csharp
public class TestLogicManager : LogicManagerBase<TestContext> {
    public NoteLogic Note { get; set; }
    public CategoryLogic Category { get; set; }

    public TestLogicManager(TestContext dbContext) : base(dbContext) { }
}

public class NoteLogic : LogicBase<TestContext, Note, Guid> {
    public NoteLogic(TestLogicManager manager, CategoryLogic categoryLogic) : base(manager) {

    }
}

public class CategoryLogic : LogicBase<TestContext, Category, Guid> {
    public CategoryLogic(TestLogicManager manager) : base(manager) {

    }
}
```

### 2.設定ConfigureServices，加入相依注入
```csharp
public void ConfigureServices(IServiceCollection services) {
    // 加入測試DbContext與內容
    services.AddScoped<TestContext>(x => TestContext.CreateInstance());

    // 加入`LogicManager`
    services.AddLogic<TestLogicManager, TestContext>()
        .AddFromDbContext(); //產生動態
}
```

### 3.於Controller建構子注入
```csharp
[Route("api/[controller]")]
public class TestController : ControllerBase {
    public TestLogicManager Manager { get; set; }
    public TestController(TestLogicManager manager) {
        Manager = manager;
    } 
    // ... something ...
}
```

### 4.在Controller的Action使用LogicManager
```csharp
[HttpGet]
public async void Test() {
    // 取得指定Entity類型的Logic類型，當然也可以使用Manager實例中的屬性取得，LogicManager的屬性支援相依注入
    var categoryLogic = Manager.GetLogicByType<Category, Guid>();
    
    // 建立實例並加入資料庫
    var category = await categoryLogic.CreateAsync(new Category() {
        Name = "Test01"
    });

    // 更新內容
    category = await categoryLogic.UpdateAsync(new Category() {
        Id = category.Id,
        Name = "Test02"
    });

    // 取得整個資料表內容，返回的資料類型為IQueryable
    var list = await categoryLogic.ListAsync(x => x.Id == category.Id);

    // 刪除資料
    await categoryLogic.DeleteAsync(category.Id);
   
    // 取得列表內容並計算資料筆數
    var listCount = await categoryLogic.ListAsync(x => x.Id == category.Id).Count();
}
```

## LogicBase掛勾
LogicBase類別提供事件處理掛勾，提供CRUD操作前後的處理，可以使用override複寫方法使用

1. 建立前處理: Task BeforeCreate(TEntity entity, params object[] parameters)
2. 建立後處理: Task BeforeUpdate(TEntity entity, params object[] parameters)
3. 刪除前處理: Task BeforeDelete(TEntity entity, params object[] parameters)
4. 取得後處理: Task AfterGet(TEntity entity, params object[] parameters)
5. 建立後處理: Task AfterCreate(TEntity entity, params object[] parameters)
6. 更新後處理: Task AfterUpdate(TEntity entity, params object[] parameters)
7. 刪除後處理: Task AfterDelete(TEntity entity, params object[] parameters)