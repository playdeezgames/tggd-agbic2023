Imports System
Imports System.IO
Imports BQ.Data
Imports Shouldly
Imports Xunit

Namespace BQ.Persistence.Tests
    Public Class World__Save__should
        <Fact>
        Sub save_to_a_file()
            Dim filename = Guid.NewGuid.ToString
            Dim subject As IWorld = New World(New WorldData)
            subject.Save(filename)
            File.ReadAllText(filename).ShouldBe("{""Maps"":[],""Characters"":[],""AvatarCharacterId"":null,""Messages"":[],""Items"":[]}")
        End Sub
    End Class
End Namespace

