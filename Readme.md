<!-- default badges list -->
[![](https://img.shields.io/badge/Open_in_DevExpress_Support_Center-FF7200?style=flat-square&logo=DevExpress&logoColor=white)](https://supportcenter.devexpress.com/ticket/details/E5125)
[![](https://img.shields.io/badge/📖_How_to_use_DevExpress_Examples-e9f6fc?style=flat-square)](https://docs.devexpress.com/GeneralInformation/403183)
<!-- default badges end -->

# Grid View for ASP.NET MVC - How to implement optimistic concurrency for update and delete operations

This example illustrates how to implement [optimistic concurrency](https://en.wikipedia.org/wiki/Optimistic_concurrency_control) when you bind the [GridView]() extension to a data source using the Entity Framework and the "Code First" development approach. See the following topic for more information: [Bind Grid View to Data via Entity Framework (Code First)](https://docs.devexpress.com/AspNetMvc/14580/components/grid-view/binding-to-data/binding-to-data-via-entity-framework-code-first).

Note that [Entity Framework](https://learn.microsoft.com/en-us/ef/ef6/) includes the corresponding functionality out-of-the-box if you define the [Timestamp](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#timestamp) field in your data model:

```cs
[Timestamp]
public Byte[] RowVersion { get; set; }
```

Once you define the `Timestamp` field, entity framework automatically performs concurrency checks when saving data to the database. In case of concurrency conflict, the [DbUpdateConcurrencyException](https://learn.microsoft.com/en-us/dotnet/api/system.data.entity.infrastructure.dbupdateconcurrencyexception) is thrown.

In stateless environments like ASP.NET MVC, you need some additional logic over the default Entity Framework's logic to keep the current `RowVersion` value for the specific object between postbacks. To pass RowVersion-related data to the client, you can use the approach illustrated in the following topic: [Passing Values Between Server and Client Sides](https://docs.devexpress.com/AspNetMvc/402316/common-features/client-side-functionality/passing-values-between-server-and-client-sides).

```cs
settings.CustomJSProperties = (s, e) => {
    MVCxGridView gridView = (MVCxGridView)s;
    var dictionary = new System.Collections.Generic.Dictionary<object, string>();
    for (int i = 0; i < gridView.SettingsPager.PageSize; i++) {
        var visibleIndex = i + gridView.VisibleStartIndex;
        if (visibleIndex >= gridView.VisibleRowCount)
            break;
        object[] rowValues = (object[])gridView.GetRowValues(visibleIndex, gridView.KeyFieldName, "RowVersion");
        dictionary[rowValues[0].ToString()] = Convert.ToBase64String((byte[])rowValues[1]);
    }
    e.Properties["cpRowVersions"] = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dictionary);
    if (ViewData["EditError"] != null)
        e.Properties["cpEditError"] = ViewData["EditError"];
};
```

After that, pass this data back to the corresponding Update or Delete controller's action method on the server side. Refer to the following topic for more information: [Passing Values to a Controller Action through Callbacks](https://docs.devexpress.com/AspNetMvc/9941/common-features/callback-based-functionality/passing-values-to-a-controller-action-through-callbacks).

```js
function GridView_BeginCallback(s, e) {
    e.customArgs['RowVersions'] = s.cpRowVersions;
}
```

On the server side, deserialize this data and pass the correct value to the `RowVersion` field of the currently updated or deleted object.

```cs
public ActionResult GridViewPartialUpdate(Customer customer) {
    var model = db.Customers;
    customer.RowVersion = CalculateOldRowVersion(customer.Id);
    if (ModelState.IsValid) {
        try {
            db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
        }
        catch (Exception e) {
            ViewData["EditError"] = e.Message;
        }
    }
    else
        ViewData["EditError"] = "Please, correct all errors.";
    return PartialView("GridViewPartial", db.Customers.ToList());
}
private byte[] CalculateOldRowVersion(int id) {
    JavaScriptSerializer serializer = new JavaScriptSerializer();
    string rowVersions = Request["RowVersions"];
    Dictionary<object, string> dictionary = (Dictionary<object, string>)serializer.Deserialize(rowVersions, typeof(Dictionary<object, string>));
    char[] rowVersion = dictionary[id.ToString()].ToCharArray();
    return Convert.FromBase64CharArray(rowVersion, 0, rowVersion.Length);
}
```

## Files to Review

* [HomeController.cs](./CS/Controllers/HomeController.cs) (VB: [HomeController.vb](./VB/Controllers/HomeController.vb))
* [Customer.cs](./CS/Models/Customer.cs) (VB: [Customer.vb](./VB/Models/Customer.vb))
* [GridViewPartial.cshtml](./CS/Views/Home/GridViewPartial.cshtml)
* [Index.cshtml](./CS/Views/Home/Index.cshtml)

## Documentation

* [Bind Grid View to Data via Entity Framework (Code First)](https://docs.devexpress.com/AspNetMvc/14580/components/grid-view/binding-to-data/binding-to-data-via-entity-framework-code-first)
* [Passing Values Between Server and Client Sides](https://docs.devexpress.com/AspNetMvc/402316/common-features/client-side-functionality/passing-values-between-server-and-client-sides)
* [Passing Values to a Controller Action through Callbacks](https://docs.devexpress.com/AspNetMvc/9941/common-features/callback-based-functionality/passing-values-to-a-controller-action-through-callbacks)
* [Timestamp](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/data-annotations#timestamp)

