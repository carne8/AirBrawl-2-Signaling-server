module SignalingServer.Tests.Signaling.All

open System
open Expecto
open Microsoft.AspNetCore.SignalR.Client
open System.Threading.Tasks

open SignalingServer.Signaling
open SignalingServer.Tests
open SignalingServer.Tests.Signaling
open SignalingServer.Tests.Signaling.Common

[<Tests>]
let signalingTests =
    let testServer, connectionAttemptStore, roomStore, playerStore = Common.createTestServer()

    testList "Signaling" [
        testTask "Signaling hub connection should success" {
            let! (hub: TestHubClient) = testServer |> connectHub
            do! Task.Delay(TimeSpan.FromSeconds 1.) // Make sure the server has the time to register the player

            Expect.equal hub.Connection.State HubConnectionState.Connected "Should be connected to the hub"

            let playerId = hub.PlayerId
            let player =
                playerStore.Get playerId
                |> Flip.Expect.wantSome "Client should be registered in the player connections store"

            Expect.equal player.Id playerId "Player ids should be the same"
        }

        WebRTCSignaling.tests testServer connectionAttemptStore
        RoomManagement.tests testServer roomStore
    ]
