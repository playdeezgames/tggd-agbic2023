﻿Imports System.Data.Common
Imports System.Runtime.CompilerServices
Imports SPLORR.Game
Imports BQ.Persistence

Friend Module MapTypes
    Friend Const Town = "Town"
    Friend Const Wilderness = "Wilderness"
    Friend Const Healer = "Healer"
    Friend Const HealthTrainer = "HealthTrainer"
    Friend Const DruidHouse = "DruidHouse"
    Friend Const Inn = "Inn"
    Friend Const EnergyTrainer = "EnergyTrainer"
    Friend Const Cellar = "Cellar"
    Friend Const Potter = "Potter"
    Private ReadOnly descriptors As IReadOnlyDictionary(Of String, MapTypeDescriptor) =
        New Dictionary(Of String, MapTypeDescriptor) From
        {
            {
                Town,
                New MapTypeDescriptor(
                    (TownColumns, TownRows),
                    "Empty",
                    customInitializer:=AddressOf TownInitializer.Initialize,
                    spawnCharacters:=New Dictionary(Of String, Integer) From
                    {
                        {"Loxy", 1}
                    })
            },
            {
                Wilderness,
                New MapTypeDescriptor(
                    (WildernessColumns, WildernessRows),
                    "Grass",
                    customInitializer:=AddressOf WildernessInitializer.Initialize)
            },
            {
                Healer,
                New MapTypeDescriptor(
                    (HealerColumns, HealerRows),
                    "Empty",
                    customInitializer:=AddressOf HealerInitializer.Initialize)
            },
            {
                HealthTrainer,
                New MapTypeDescriptor(
                    (HealthTrainerColumns, HealthTrainerRows),
                    "Empty",
                    customInitializer:=AddressOf HealthTrainerInitializer.Initialize)
            },
            {
                Potter,
                New MapTypeDescriptor(
                    (PotterColumns, PotterRows),
                    "Empty",
                    customInitializer:=AddressOf PotterInitializer.Initialize)
            },
            {
                DruidHouse,
                New MapTypeDescriptor(
                    (DruidHouseColumns, DruidHouseRows),
                    "Empty",
                    customInitializer:=AddressOf DruidHouseInitializer.Initialize)
            },
            {
                Inn,
                New MapTypeDescriptor(
                    (InnColumns, InnRows),
                    "Empty",
                    customInitializer:=AddressOf InnInitializer.Initialize)
            },
            {
                Cellar,
                New MapTypeDescriptor(
                    (CellarColumns, CellarRows),
                    "CellarFloor",
                    customInitializer:=AddressOf CellarInitializer.Initialize)
            },
            {
                EnergyTrainer,
                New MapTypeDescriptor(
                    (EnergyTrainerColumns, EnergyTrainerRows),
                    "Empty",
                    customInitializer:=AddressOf EnergyTrainerInitializer.Initialize)
            }
        }

    <Extension>
    Friend Function ToMapTypeDescriptor(mapType As String) As MapTypeDescriptor
        Return descriptors(mapType)
    End Function
    Friend ReadOnly Property All As IEnumerable(Of String)
        Get
            Return descriptors.Keys
        End Get
    End Property
End Module
