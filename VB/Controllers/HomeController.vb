Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web.Mvc
Imports System.Web.Script.Serialization
Imports GridViewOptimisticConcurrencyMvc.Models

Namespace GridViewOptimisticConcurrencyMvc.Controllers
    Public Class HomeController
        Inherits Controller
        Private db As New CustomerDbContext()

        Public Function Index() As ActionResult
            Return View()
        End Function

        Public Function GridViewPartial() As ActionResult
            Return PartialView(db.Customers.ToList())
        End Function

        Public Function GridViewPartialAddNew(ByVal customer As Customer) As ActionResult
            Dim model = db.Customers

            If ModelState.IsValid Then
                Try
                    db.Entry(customer).State = System.Data.Entity.EntityState.Added
                    db.SaveChanges()
                Catch e As Exception
                    ViewData("EditError") = e.Message
                End Try
            Else
                ViewData("EditError") = "Please, correct all errors."
            End If

            Return PartialView("GridViewPartial", db.Customers.ToList())
        End Function

        Public Function GridViewPartialUpdate(ByVal customer As Customer) As ActionResult
            Dim model = db.Customers

            customer.RowVersion = CalculateOldRowVersion(customer.Id)

            If ModelState.IsValid Then
                Try
                    db.Entry(customer).State = System.Data.Entity.EntityState.Modified
                    db.SaveChanges()
                Catch e As Exception
                    ViewData("EditError") = e.Message
                End Try
            Else
                ViewData("EditError") = "Please, correct all errors."
            End If

            Return PartialView("GridViewPartial", db.Customers.ToList())
        End Function

        Public Function GridViewPartialDelete(ByVal customer As Customer) As ActionResult
            Dim model = db.Customers

            customer.RowVersion = CalculateOldRowVersion(customer.Id)

            If ModelState.IsValid Then
                Try
                    db.Entry(customer).State = System.Data.Entity.EntityState.Deleted
                    db.SaveChanges()
                Catch e As Exception
                    ViewData("EditError") = e.Message
                End Try
            Else
                ViewData("EditError") = "Please, correct all errors."
            End If

            Return PartialView("GridViewPartial", db.Customers.ToList())
        End Function

        Private Function CalculateOldRowVersion(ByVal id As Integer) As Byte()
            Dim serializer As New JavaScriptSerializer()
            Dim rowVersions As String = Request("RowVersions")
            Dim dictionary As Dictionary(Of Object, String) = CType(serializer.Deserialize(rowVersions, GetType(Dictionary(Of Object, String))), Dictionary(Of Object, String))
            Dim rowVersion() As Char = dictionary(id.ToString()).ToCharArray()

            Return Convert.FromBase64CharArray(rowVersion, 0, rowVersion.Length)
        End Function
    End Class
End Namespace