@ModelType System.Collections.IEnumerable

@Html.DevExpress().GridView( _
    Sub(settings)
            settings.Name = "GridView"
            settings.KeyFieldName = "Id"
            
            settings.CallbackRouteValues = New With {Key .Controller = "Home", Key .Action = "GridViewPartial"}
            
            settings.SettingsEditing.AddNewRowRouteValues = New With {Key .Controller = "Home", Key .Action = "GridViewPartialAddNew"}
            settings.SettingsEditing.UpdateRowRouteValues = New With {Key .Controller = "Home", Key .Action = "GridViewPartialUpdate"}
            settings.SettingsEditing.DeleteRowRouteValues = New With {Key .Controller = "Home", Key .Action = "GridViewPartialDelete"}
            
            settings.CommandColumn.Visible = True
            settings.CommandColumn.ShowEditButton = True
            settings.CommandColumn.ShowNewButton = True
            settings.CommandColumn.ShowDeleteButton = True

            settings.Columns.Add( _
                Sub(column)
                        column.FieldName = "Id"
                        column.ReadOnly = True
                        column.EditFormSettings.Visible = DefaultBoolean.False
                End Sub)

            settings.Columns.Add("FirstName")
            settings.Columns.Add("LastName")
            settings.Columns.Add("Age")
            settings.Columns.Add("Email")

            settings.CustomJSProperties = _
                 Sub(s, e)
                         Dim gridView As MVCxGridView = CType(s, MVCxGridView)
      
                         Dim dictionary = New System.Collections.Generic.Dictionary(Of Object, String)()

                         For i As Integer = 0 To gridView.SettingsPager.PageSize - 1
                             Dim visibleIndex = i + gridView.VisibleStartIndex
                             If (visibleIndex >= gridView.VisibleRowCount) Then
                                 Exit For
                             End If

                             Dim rowValues() As Object = CType(gridView.GetRowValues(visibleIndex, gridView.KeyFieldName, "RowVersion"), Object())

                             dictionary(rowValues(0).ToString()) = Convert.ToBase64String(CType(rowValues(1), Byte()))
                         Next i

                         e.Properties("cpRowVersions") = New System.Web.Script.Serialization.JavaScriptSerializer().Serialize(dictionary)

                         If ViewData("EditError") IsNot Nothing Then
                             e.Properties("cpEditError") = ViewData("EditError")
                             
                         End If
        
                 End Sub

            settings.ClientSideEvents.BeginCallback = "GridView_BeginCallback"
            settings.ClientSideEvents.EndCallback = "GridView_EndCallback"
            
    End Sub).SetEditErrorText(CStr(ViewData("EditError"))).Bind(Model).GetHtml()