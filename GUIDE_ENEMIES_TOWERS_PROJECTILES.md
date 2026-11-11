# HƯỚNG DẪN TẠO PREFAB & DATA (ENEMIES - TOWERS - PROJECTILES)

---

### BƯỚC 1: TẠO CÁC PREFAB ĐẠN NÂNG CẤP (UPGRADED PROJECTILES)
*Thư mục lưu Prefab: `Assets/Prefabs/Projectiles/`*  
*Thư mục ảnh sprite: `Assets/Art/Projectiles/`*  
*(4 đạn cấp 1 đã có sẵn: `CanonShot`, `LaserShot`, `GlueShot`, `MortarShot`).*

#### Các bước thực hiện:
1. Chọn file **`CanonShot.prefab`** $\rightarrow$ bấm **`Ctrl + D`** nhân bản ra từng file đạn nâng cấp.
2. Mở từng Prefab, sổ mũi tên file ảnh trong `Assets/Art/Projectiles/` lấy **khung hình `_0`** kéo vào `SpriteRenderer -> Sprite`.
3. Điền cấu hình trên component **`Projectile`**:

| Tên Prefab Đạn | Dành cho Tháp | File Sprite (Khung hình 0) | Scale | Cấu hình trên script `Projectile` |
| :--- | :--- | :--- | :---: | :--- |
| **`DualCanonShot`** | **DualCanon** | `canon_dual_shot_0` | `(3, 3, 1)` | Bỏ tích cả `Is AOE` và `Is Glue`. |
| **`MachineGunShot`** | **MachineGun** | `canon_mg_shot_0` | `(2.5, 2.5, 1)` | Bỏ tích cả `Is AOE` và `Is Glue`. |
| **`StraightLaserShot`**| **StraightLaser**| `laser_straight_effect_0`| `(2.5, 2.5, 1)` | Bỏ tích cả `Is AOE` và `Is Glue`. |
| **`BouncingLaserShot`**| **BouncingLaser**| `laser_bouncing_effect_0`| `(2.5, 2.5, 1)` | Bỏ tích cả `Is AOE` và `Is Glue`. |
| **`GlueFocusedShot`** | **GlueTower** | `glue_focused_shot_0` | `(3, 3, 1)` | **`Is Glue` = `true`** • `Slow Percent`: **`0.7`** • `Slow Duration`: **`3.5`** |
| **`RocketShot`** | **RocketLauncher**| `rocket_0` | `(3, 3, 1)` | **`Is AOE` = `true`** • `AOE Radius`: **`1.8`** |
| **`MineShot`** | **MineLayer** | `mine_0` | `(3, 3, 1)` | **`Is AOE` = `true`** • `AOE Radius`: **`2.2`** |

> **Kiểm tra bắt buộc trên toàn bộ Prefab đạn:**
> * `SpriteRenderer -> Order in Layer` = **`10`**.
> * `CircleCollider2D -> Is Trigger` = **`true`**.
> * `Rigidbody2D -> Body Type` = **`Kinematic`**.
> * `Projectile -> Hit Effect Prefab` = kéo **`HitSparks.prefab`** vào.

---

### BƯỚC 2: TẠO SCRIPTABLE OBJECT DỮ LIỆU THÁP NÂNG CẤP (TOWER DATA)
*Thư mục lưu trữ: `Assets/Data/Towers/`*  
*(4 tháp cấp 1 đã có: `Tower_Canon_Data`, `Tower_Laser_Data`, `Tower_Glue_Data`, `Tower_Mortar_Data`).*

#### Các bước thực hiện:
1. Click chuột phải $\rightarrow$ chọn **`Create -> Anuto TD -> Tower Data`**, tạo **8 file** cho các cấp nâng cấp:
   * **`Tower_DualCanon_Data`**
   * **`Tower_MachineGun_Data`**
   * **`Tower_StraightLaser_Data`**
   * **`Tower_BouncingLaser_Data`**
   * **`Tower_GlueTower_Data`**
   * **`Tower_Teleporter_Data`**
   * **`Tower_RocketLauncher_Data`**
   * **`Tower_MineLayer_Data`**
2. Mở từng file và điền thông số trong **Inspector** theo đúng bảng chuẩn:

| Tên File Data | Tower Type (Enum) | Gun Sprite | Cost | Attack Range | Fire Rate | Damage | Proj Speed | Projectile Prefab (Kéo file đạn ở Bước 1 vào) |
| :--- | :--- | :--- | :---: | :---: | :---: | :---: | :---: | :--- |
| **`Tower_DualCanon_Data`** | `DualCanon` | `canon_dual_0` | `220` | `3.8` | `2.2` | `50` | `14` | `DualCanonShot.prefab` |
| **`Tower_MachineGun_Data`** | `MachineGun` | `canon_mg_0` | `400` | `4.0` | `5.0` | `30` | `16` | `MachineGunShot.prefab` |
| **`Tower_StraightLaser_Data`**| `StraightLaser` | `laser_tower2_0` | `280` | `3.5` | `3.5` | `25` | `25` | `StraightLaserShot.prefab` |
| **`Tower_BouncingLaser_Data`**| `BouncingLaser` | `laser_tower3_0` | `480` | `4.2` | `4.0` | `35` | `28` | `BouncingLaserShot.prefab` |
| **`Tower_GlueTower_Data`** | `GlueTower` | `glue_tower_full_0`| `250` | `3.2` | `1.4` | `18` | `12` | `GlueFocusedShot.prefab` |
| **`Tower_Teleporter_Data`** | `Teleporter` | `teleport_tower_0` | `450` | `3.5` | `0.8` | `25` | `12` | `GlueFocusedShot.prefab` |
| **`Tower_RocketLauncher_Data`**| `RocketLauncher`| `rocket_launcher_0`| `350` | `5.2` | `0.9` | `120` | `11` | `RocketShot.prefab` |
| **`Tower_MineLayer_Data`** | `MineLayer` | `mine_layer_0` | `420` | `4.0` | `0.5` | `180` | `7` | `MineShot.prefab` |

---

### BƯỚC 3: TẠO CÁC PREFAB THÁP NÂNG CẤP (TOWERS PREFAB)
*Thư mục lưu trữ: `Assets/Prefabs/Towers/`*  
*(4 tháp cấp 1 đã có: `Tower_Canon`, `Tower_Laser`, `Tower_Glue`, `Tower_Mortar`).*

#### Các bước thực hiện:
1. Chọn file `Tower_Canon.prefab` có sẵn $\rightarrow$ bấm **`Ctrl + D`** nhân bản ra **8 Prefab tháp nâng cấp**:
   * **`Tower_DualCanon`**
   * **`Tower_MachineGun`**
   * **`Tower_StraightLaser`**
   * **`Tower_BouncingLaser`**
   * **`Tower_GlueTower`**
   * **`Tower_Teleporter`**
   * **`Tower_RocketLauncher`**
   * **`Tower_MineLayer`**
2. Double-click mở từng Prefab để chỉnh 3 mục:
   * **Mục 1 (`SpriteRenderer`)**: Kéo sprite khung hình 0 tương ứng từ `Assets/Art/Towers/` vào ô `Sprite` (`canon_dual_0`, `canon_mg_0`...). Đảm bảo `Order in Layer = 4`.
   * **Mục 2 (`TowerController`)**:
     * `Tower Data`: Kéo file SO tương ứng ở Bước 2 vào.
     * `Gun Renderer`: Kéo SpriteRenderer của chính nó vào.
     * `Fire Point`: **Để trống `None`** (code tự động tìm đối tượng con).
     * `Rotation Offset`: Đặt **`-90`**.
   * **Mục 3 (`FirePoint` con)**: Chọn đối tượng con `FirePoint`, dùng phím **`W`** kéo vị trí chấm tròn lên ngay đỉnh đầu miệng nòng súng của tháp đó.

---

### BƯỚC 4: TẠO SCRIPTABLE OBJECT DỮ LIỆU QUÁI (ENEMY DATA)
*Thư mục lưu trữ: `Assets/Data/Enemies/`*

1. Click chuột phải $\rightarrow$ chọn **`Create -> Anuto TD -> Enemy Data`**, tạo các file:
   * **`Enemy_Soldier_Data`** *(nếu chưa có)*
   * **`Enemy_Blob_Data`**
   * **`Enemy_Sprinter_Data`**
   * **`Enemy_ShieldBearer_Data`**
   * **`Enemy_Flyer_Data`**
   * **`Enemy_Boss_Data`**
2. Điền thông số vào Inspector cho từng file:

| File Data | Enemy Type | Sprite Avatar | Max Health | Move Speed | Reward | Damage To Base | Ghi chú thêm |
| :--- | :--- | :--- | :---: | :---: | :---: | :---: | :--- |
| **`Enemy_Soldier_Data`**| `Soldier` | Frame 0 Soldier | `100` | `2.5` | `15` | `1` | Quái cân bằng |
| **`Enemy_Blob_Data`** | `Blob` | Frame 0 Blob | `250` | `1.2` | `25` | `1` | Máu trâu, chạy chậm |
| **`Enemy_Sprinter_Data`**| `Sprinter`| Frame 0 Sprinter | `60` | `4.2` | `20` | `1` | Tốc độ cao, máu mỏng |
| **`Enemy_ShieldBearer_Data`**| `ShieldBearer`| Frame 0 Khiên | `160` | `2.0` | `30` | `1` | `Physical Armor = 10` |
| **`Enemy_Flyer_Data`** | `Flyer` | Frame 0 Bay | `120` | `3.0` | `35` | `1` | `Is Flying = true` |
| **`Enemy_Boss_Data`** | `Boss` | Frame 0 Boss | `1500` | `0.9` | `250` | `5` | Boss chiến dịch |

---

### BƯỚC 5: TẠO PREFAB QUÁI VẬT (ENEMIES PREFAB)
*Thư mục lưu trữ: `Assets/Prefabs/Enemies/`*

1. Chọn `Enemy_Soldier.prefab` có sẵn $\rightarrow$ bấm **`Ctrl + D`** tạo các file:
   * **`Enemy_Blob`**
   * **`Enemy_Sprinter`**
   * **`Enemy_ShieldBearer`**
   * **`Enemy_Flyer`**
   * **`Enemy_Boss`**
2. Double click mở từng Prefab:
   * `SpriteRenderer`: Đổi sang ảnh frame 0 của quái tương ứng (`Order in Layer = 5`).
   * `EnemyController -> Enemy Data`: Kéo file SO tương ứng ở Bước 4 vào.
   * `EnemyController -> Rotation Offset`: Giữ nguyên **`90`**.
   * `CircleCollider2D`: Kiểm tra **`Is Trigger = true`**, căn chỉnh bán kính `Radius` vừa vặn thân quái.
   * `Animator` *(nếu có)*: Gán Controller hoạt ảnh bước đi tương ứng.
