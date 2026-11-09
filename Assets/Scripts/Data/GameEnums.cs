namespace AnutoTD.Data
{
    // 12 loại tháp thuộc 4 nhánh
    public enum TowerType
    {
        // Nhánh Súng
        Canon,
        DualCanon,
        MachineGun,

        // Nhánh Laser
        SimpleLaser,
        StraightLaser,
        BouncingLaser,

        // Nhánh Làm chậm
        GlueGun,
        GlueTower,
        Teleporter,

        // Nhánh Nổ
        Mortar,
        RocketLauncher,
        MineLayer
    }

    // Các loại quái
    public enum EnemyType
    {
        Blob,
        Soldier,
        Sprinter,
        ShieldBearer,
        Flyer,
        Healer,
        Saboteur,
        Boss,
        WitchBoss
    }

    // 5 chiến lược nhắm bắn của tháp
    public enum TargetStrategy
    {
        First,      // Đi xa nhất
        Last,       // Mới ra
        Closest,    // Gần tháp nhất
        Strongest,  // Máu nhiều nhất
        Weakest     // Máu ít nhất
    }
}