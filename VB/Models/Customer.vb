Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.ComponentModel.DataAnnotations.Schema
Imports System.Data.Entity

Namespace GridViewOptimisticConcurrencyMvc.Models
	Public Class Customer
		Private privateId As Integer
		<Key> _
		Public Property Id() As Integer
			Get
				Return privateId
			End Get
			Set(ByVal value As Integer)
				privateId = value
			End Set
		End Property
		Private privateFirstName As String
		Public Property FirstName() As String
			Get
				Return privateFirstName
			End Get
			Set(ByVal value As String)
				privateFirstName = value
			End Set
		End Property
		Private privateLastName As String
		Public Property LastName() As String
			Get
				Return privateLastName
			End Get
			Set(ByVal value As String)
				privateLastName = value
			End Set
		End Property
		Private privateAge As Integer
		Public Property Age() As Integer
			Get
				Return privateAge
			End Get
			Set(ByVal value As Integer)
				privateAge = value
			End Set
		End Property
		Private privateEmail As String
		Public Property Email() As String
			Get
				Return privateEmail
			End Get
			Set(ByVal value As String)
				privateEmail = value
			End Set
		End Property
		Private privateRowVersion As Byte()
		<Timestamp> _
		Public Property RowVersion() As Byte()
			Get
				Return privateRowVersion
			End Get
			Set(ByVal value As Byte())
				privateRowVersion = value
			End Set
		End Property
	End Class

	Public Class CustomerDbContext
		Inherits DbContext
		Public Sub New()
			MyBase.New("CustomerDbContext")
			Database.SetInitializer(New CustomerDbContextInitializer())
		End Sub

		Private privateCustomers As DbSet(Of Customer)
		Public Property Customers() As DbSet(Of Customer)
			Get
				Return privateCustomers
			End Get
			Set(ByVal value As DbSet(Of Customer))
				privateCustomers = value
			End Set
		End Property
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
			Next std

			MyBase.Seed(context)
		End Sub
	End Class
End Namespace