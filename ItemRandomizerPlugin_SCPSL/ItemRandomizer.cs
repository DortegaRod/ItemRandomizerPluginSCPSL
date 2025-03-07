namespace ItemRandomizerPlugin {

    using Exiled.API.Features;
    using ItemRandomizerPlugin_SCPSL;
    using System;
    using Player = Exiled.Events.Handlers.Player;
    using Server = Exiled.Events.Handlers.Server;
    using Map = Exiled.Events.Handlers.Map;
    using Exiled.Events.EventArgs.Map;

    public class ItemRandomizer : Plugin<Config> {
        public PlayerHandler _playerHandler;
        public override string Name => "ItemRandomizerPlugin";
        public override string Prefix => "IRndPl";
        public override string Author => "Megador";
        public override Version Version => new Version(1, 0, 0);
        public override Version RequiredExiledVersion => new Version(8, 9, 11);

        public override void OnEnabled() {
            Log.Info("ItemRandomizerPlugin loaded successfully");
            _playerHandler = new PlayerHandler();
            Player.DroppingItem += _playerHandler.OnItemDropped;
            Player.FlippingCoin += _playerHandler.OnFlip;
            Player.Spawned += _playerHandler.OnSpawned;
            Server.RoundEnded += _playerHandler.ClearCoinList;
            Map.Decontaminating += OnDecontaminating;
            base.OnEnabled();
        }

        public override void OnDisabled() {
            Player.DroppingItem -= _playerHandler.OnItemDropped;
            Player.FlippingCoin -= _playerHandler.OnFlip;
            Player.Spawned += _playerHandler.OnSpawned;
            Server.RoundEnded -= _playerHandler.ClearCoinList;
            Map.Decontaminating -= OnDecontaminating;
            _playerHandler = null;
            base.OnDisabled();
        }

        public void OnDecontaminating(DecontaminatingEventArgs ev)
        {
            Exiled.API.Features.Map.Broadcast(10, "COIN TP ENABLED IN ALL THE FACILITY");
        }
    }
}