Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity

Namespace GridViewOptimisticConcurrencyMvc.Models

    Public Class Customer

        <Key>
        Public Property Id As Integer

        Public Property FirstName As String

        Public Property LastName As String

        Public Property Age As Integer

        Public Property Email As String

        <Timestamp>
        Public Property RowVersion As Byte()
    End Class

    Public Class CustomerDbContext
        Inherits DbContext

        Public Sub New()
            MyBase.New("CustomerDbContext")
            Call Database.SetInitializer(New CustomerDbContextInitializer())
        End Sub

        Public Property Customers As DbSet(Of Customer)
    End Class

    Public Class CustomerDbContextInitializer
        Inherits DropCreateDatabaseIfModelChanges(Of CustomerDbContext)

        Protected Overrides Sub Seed(ByVal context As CustomerDbContext)
            Dim defaultCustomers As IList(Of Customer) = New List(Of Customer)()
            defaultCustomers.Add(New Customer() With {.FirstName = "David", .LastName = "Adler", .Age = 25, .Email = "David.Adler@somewhere.com"})
            defaultCustomers.Add(New Customer() With {.FirstName = "Michael", .LastName = "Alcamo", .Age = 38, .Email = "Michael.Alcamo@somewhere.com"})
            defaultCustomers.Add(New Customer() With {.FirstName = "Amy", .LastName = "Altmann", .Age = 27, .Email = "Amy.Altmann@somewhere.com"})
            For Each std As Customer In defaultCustomers
                context.Customers.Add(std)
            Next

            MyBase.Seed(context)
        End Sub
    End Class
End Namespace
