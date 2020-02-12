﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace QSB {
    public class NetworkPlayer: NetworkBehaviour {
        Transform _body;
        float _smoothSpeed = 10f;
        public static NetworkPlayer localInstance { get; private set; }

        void Start () {
            if (isLocalPlayer) {
                QSB.LogToScreen("Started LOCAL network player", netId.Value);
            } else {
                QSB.LogToScreen("Started REMOTE network player", netId.Value);
            }
            QSB.playerSectors[netId.Value] = Locator.GetAstroObject(AstroObject.Name.TimberHearth).transform;

            var player = Locator.GetPlayerBody().transform.Find("Traveller_HEA_Player_v2");
            if (isLocalPlayer) {
                localInstance = this;
                _body = player;
            } else {
                _body = Instantiate(player);
                _body.GetComponent<PlayerAnimController>().enabled = false;
                _body.Find("player_mesh_noSuit:Traveller_HEA_Player/player_mesh_noSuit:Player_Head").gameObject.layer = 0;
                _body.parent = transform;
                _body.localPosition = Vector3.zero;
                _body.localRotation = Quaternion.identity;
            }

            // It's dumb that this is here, should be somewhere else.
            if (isServer) {
                NetworkServer.RegisterHandler(MsgType.Highest + 1, QSB.OnReceiveMessage);
            } else {
                NetworkManager.singleton.client.RegisterHandler(SectorMessage.Type, QSB.OnReceiveMessage);
            }

        }

        public void EnterSector (Sector sector) {
            var name = sector.GetName();
            if (name != Sector.Name.Unnamed && name != Sector.Name.Ship) {
                QSB.playerSectors[netId.Value] = QSB.GetSectorByName(sector.GetName());

                SectorMessage msg = new SectorMessage();
                msg.sectorId = (int) sector.GetName();
                msg.senderId = netId.Value;
                if (isServer) {
                    NetworkServer.SendToAll(SectorMessage.Type, msg);
                } else {
                    connectionToServer.Send(SectorMessage.Type, msg);
                }
            }
        }

        void Update () {
            if (!_body) {
                return;
            }

            var sectorTransform = QSB.playerSectors[netId.Value];
            if (isLocalPlayer) {
                transform.position = sectorTransform.InverseTransformPoint(_body.position);
                transform.rotation = sectorTransform.InverseTransformRotation(_body.rotation);
            } else {
                //var lerpPosition = Vector3.Lerp(_body.position, sectorTransform.TransformPoint(transform.position), _smoothSpeed * Time.deltaTime);
                _body.position = sectorTransform.TransformPoint(transform.position);
                _body.rotation = sectorTransform.rotation * transform.rotation;
            }
        }
    }
}
