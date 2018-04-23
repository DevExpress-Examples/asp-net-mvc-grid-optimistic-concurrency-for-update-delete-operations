# GridView - How to implement optimistic concurrency for the update/delete operations


<p>This example illustrates how to realize <a href="http://en.wikipedia.org/wiki/Optimistic_concurrency_control"><u>optimistic concurrency</u></a> when <a href="http://documentation.devexpress.com/#AspNet/CustomDocument14580"><u>Binding to Data via Entity Framework (Code First)</u></a>. Note that <a href="http://msdn.microsoft.com/en-us/data/aa937723"><u>Entity Framework</u></a> includes the corresponding functionality out-of-the-box. You just need to define the following field in your data model:</p>

```cs
        [Timestamp]
        public Byte[] RowVersion { get; set; }
```

<p> </p><p>Check the corresponding section in the <a href="http://msdn.microsoft.com/en-us/data/jj591583.aspx"><u>Code First Data Annotations</u></a> to learn more on this subject. Once you define this field, entity framework will automatically perform concurrency checks when saving data to the database. The <a href="http://msdn.microsoft.com/en-us/library/system.data.entity.infrastructure.dbupdateconcurrencyexception.aspx"><u>DbUpdateConcurrencyException</u></a> will be thrown in case of concurrency conflict.</p><p>This scenario is quite easy to implement in stateful environments like WinForms. But in the case of a stateless ASP.NET MVC environment, you need some additional logic over the default Entity Framework's logic to keep the current <strong>RowVersion</strong> value for the specific object between postbacks. In a regular MVC project, it is possible to use a hidden field for this purpose (e.g., see <a href="http://www.asp.net/mvc/tutorials/getting-started-with-ef-5-using-mvc-4/handling-concurrency-with-the-entity-framework-in-an-asp-net-mvc-application"><u>Handling Concurrency with the Entity Framework in an ASP.NET MVC Application (7 of 10)</u></a>). In case of our MVC extensions you can use the approach illustrated in the <a href="http://documentation.devexpress.com/#AspNet/CustomDocument11816"><u>How to: Access Server Data on the Client Side</u></a> document to pass RowVersion-related data on the client side:</p>

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

<p> </p><p>After that, pass this data back to the corresponding Update/Delete controller's action method on the server side by using the approach from the <a href="http://documentation.devexpress.com/#AspNet/CustomDocument9941"><u>Passing Values to Controller Action Through Callbacks</u></a> document:</p>

```js
    function GridView_BeginCallback(s, e) {
        e.customArgs['RowVersions'] = s.cpRowVersions;
    }
```

<p> </p><p>On the server side, deserialize this data and pass the correct value to the RowVersion field of the currently updated/deleted object. Here is an example (for Update action):</p>

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



<br/>


