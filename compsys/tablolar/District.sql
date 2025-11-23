CREATE TABLE District (
    DistrictID INT IDENTITY(1,1) PRIMARY KEY,
    CityID INT NOT NULL FOREIGN KEY REFERENCES City(CityID),
    DistrictName NVARCHAR(100) NOT NULL
);
-- 81 İlin En Kalabalık 3 İlçesi

-- 1 Adana
INSERT INTO District (CityID, DistrictName) VALUES
(1, 'Seyhan'), (1, 'Yüreğir'), (1, 'Çukurova');
-- 2 Adıyaman
INSERT INTO District (CityID, DistrictName) VALUES
(2, 'Merkez'), (2, 'Kahta'), (2, 'Besni');
-- 3 Afyonkarahisar
INSERT INTO District (CityID, DistrictName) VALUES
(3, 'Merkez'), (3, 'Sandıklı'), (3, 'Dinar');
-- 4 Ağrı
INSERT INTO District (CityID, DistrictName) VALUES
(4, 'Merkez'), (4, 'Patnos'), (4, 'Doğubayazıt');
-- 5 Amasya
INSERT INTO District (CityID, DistrictName) VALUES
(5, 'Merkez'), (5, 'Merzifon'), (5, 'Suluova');
-- 6 Ankara
INSERT INTO District (CityID, DistrictName) VALUES
(6, 'Çankaya'), (6, 'Keçiören'), (6, 'Yenimahalle');
-- 7 Antalya
INSERT INTO District (CityID, DistrictName) VALUES
(7, 'Kepez'), (7, 'Muratpaşa'), (7, 'Alanya');
-- 8 Artvin
INSERT INTO District (CityID, DistrictName) VALUES
(8, 'Merkez'), (8, 'Hopa'), (8, 'Arhavi');
-- 9 Aydın
INSERT INTO District (CityID, DistrictName) VALUES
(9, 'Efeler'), (9, 'Nazilli'), (9, 'Söke');
-- 10 Balıkesir
INSERT INTO District (CityID, DistrictName) VALUES
(10, 'Karesi'), (10, 'Altıeylül'), (10, 'Bandırma');
-- 11 Bilecik
INSERT INTO District (CityID, DistrictName) VALUES
(11, 'Merkez'), (11, 'Bozüyük'), (11, 'Osmaneli');
-- 12 Bingöl
INSERT INTO District (CityID, DistrictName) VALUES
(12, 'Merkez'), (12, 'Genç'), (12, 'Solhan');
-- 13 Bitlis
INSERT INTO District (CityID, DistrictName) VALUES
(13, 'Tatvan'), (13, 'Merkez'), (13, 'Ahlat');
-- 14 Bolu
INSERT INTO District (CityID, DistrictName) VALUES
(14, 'Merkez'), (14, 'Gerede'), (14, 'Mengen');
-- 15 Burdur
INSERT INTO District (CityID, DistrictName) VALUES
(15, 'Merkez'), (15, 'Bucak'), (15, 'Gölhisar');
-- 16 Bursa
INSERT INTO District (CityID, DistrictName) VALUES
(16, 'Osmangazi'), (16, 'Yıldırım'), (16, 'Nilüfer');
-- 17 Çanakkale
INSERT INTO District (CityID, DistrictName) VALUES
(17, 'Merkez'), (17, 'Biga'), (17, 'Çan');
-- 18 Çankırı
INSERT INTO District (CityID, DistrictName) VALUES
(18, 'Merkez'), (18, 'Çerkeş'), (18, 'Şabanözü');
-- 19 Çorum
INSERT INTO District (CityID, DistrictName) VALUES
(19, 'Merkez'), (19, 'Sungurlu'), (19, 'Osmancık');
-- 20 Denizli
INSERT INTO District (CityID, DistrictName) VALUES
(20, 'Pamukkale'), (20, 'Merkezefendi'), (20, 'Acıpayam');
-- 21 Diyarbakır
INSERT INTO District (CityID, DistrictName) VALUES
(21, 'Bağlar'), (21, 'Kayapınar'), (21, 'Yenişehir');
-- 22 Edirne
INSERT INTO District (CityID, DistrictName) VALUES
(22, 'Merkez'), (22, 'Keşan'), (22, 'Uzunköprü');
-- 23 Elazığ
INSERT INTO District (CityID, DistrictName) VALUES
(23, 'Merkez'), (23, 'Kovancılar'), (23, 'Karakoçan');
-- 24 Erzincan
INSERT INTO District (CityID, DistrictName) VALUES
(24, 'Merkez'), (24, 'Üzümlü'), (24, 'Refahiye');
-- 25 Erzurum
INSERT INTO District (CityID, DistrictName) VALUES
(25, 'Yakutiye'), (25, 'Palandöken'), (25, 'Aziziye');
-- 26 Eskişehir
INSERT INTO District (CityID, DistrictName) VALUES
(26, 'Odunpazarı'), (26, 'Tepebaşı'), (26, 'Sivrihisar');
-- 27 Gaziantep
INSERT INTO District (CityID, DistrictName) VALUES
(27, 'Şahinbey'), (27, 'Şehitkamil'), (27, 'Nizip');
-- 28 Giresun
INSERT INTO District (CityID, DistrictName) VALUES
(28, 'Merkez'), (28, 'Bulancak'), (28, 'Tirebolu');
-- 29 Gümüşhane
INSERT INTO District (CityID, DistrictName) VALUES
(29, 'Merkez'), (29, 'Kelkit'), (29, 'Şiran');
-- 30 Hakkari
INSERT INTO District (CityID, DistrictName) VALUES
(30, 'Yüksekova'), (30, 'Merkez'), (30, 'Şemdinli');
-- 31 Hatay
INSERT INTO District (CityID, DistrictName) VALUES
(31, 'Antakya'), (31, 'İskenderun'), (31, 'Defne');
-- 32 Isparta
INSERT INTO District (CityID, DistrictName) VALUES
(32, 'Merkez'), (32, 'Yalvaç'), (32, 'Eğirdir');
-- 33 Mersin
INSERT INTO District (CityID, DistrictName) VALUES
(33, 'Tarsus'), (33, 'Toroslar'), (33, 'Akdeniz');
-- 34 İstanbul
INSERT INTO District (CityID, DistrictName) VALUES
(34, 'Esenyurt'), (34, 'Küçükçekmece'), (34, 'Bağcılar');
-- 35 İzmir
INSERT INTO District (CityID, DistrictName) VALUES
(35, 'Karabağlar'), (35, 'Bornova'), (35, 'Buca');
-- 36 Kars
INSERT INTO District (CityID, DistrictName) VALUES
(36, 'Merkez'), (36, 'Kağızman'), (36, 'Sarıkamış');
-- 37 Kastamonu
INSERT INTO District (CityID, DistrictName) VALUES
(37, 'Merkez'), (37, 'Tosya'), (37, 'Taşköprü');
-- 38 Kayseri
INSERT INTO District (CityID, DistrictName) VALUES
(38, 'Melikgazi'), (38, 'Kocasinan'), (38, 'Talas');
-- 39 Kırklareli
INSERT INTO District (CityID, DistrictName) VALUES
(39, 'Merkez'), (39, 'Lüleburgaz'), (39, 'Babaeski');
-- 40 Kırşehir
INSERT INTO District (CityID, DistrictName) VALUES
(40, 'Merkez'), (40, 'Kaman'), (40, 'Mucur');
-- 41 Kocaeli
INSERT INTO District (CityID, DistrictName) VALUES
(41, 'İzmit'), (41, 'Gebze'), (41, 'Darıca');
-- 42 Konya
INSERT INTO District (CityID, DistrictName) VALUES
(42, 'Selçuklu'), (42, 'Meram'), (42, 'Karatay');
-- 43 Kütahya
INSERT INTO District (CityID, DistrictName) VALUES
(43, 'Merkez'), (43, 'Tavşanlı'), (43, 'Simav');
-- 44 Malatya
INSERT INTO District (CityID, DistrictName) VALUES
(44, 'Yeşilyurt'), (44, 'Battalgazi'), (44, 'Doğanşehir');
-- 45 Manisa
INSERT INTO District (CityID, DistrictName) VALUES
(45, 'Yunusemre'), (45, 'Şehzadeler'), (45, 'Turgutlu');
-- 46 Kahramanmaraş
INSERT INTO District (CityID, DistrictName) VALUES
(46, 'Onikişubat'), (46, 'Dulkadiroğlu'), (46, 'Elbistan');
-- 47 Mardin
INSERT INTO District (CityID, DistrictName) VALUES
(47, 'Kızıltepe'), (47, 'Artuklu'), (47, 'Midyat');
-- 48 Muğla
INSERT INTO District (CityID, DistrictName) VALUES
(48, 'Bodrum'), (48, 'Fethiye'), (48, 'Menteşe');
-- 49 Muş
INSERT INTO District (CityID, DistrictName) VALUES
(49, 'Merkez'), (49, 'Bulanık'), (49, 'Malazgirt');
-- 50 Nevşehir
INSERT INTO District (CityID, DistrictName) VALUES
(50, 'Merkez'), (50, 'Avanos'), (50, 'Ürgüp');
-- 51 Niğde
INSERT INTO District (CityID, DistrictName) VALUES
(51, 'Merkez'), (51, 'Bor'), (51, 'Ulukışla');
-- 52 Ordu
INSERT INTO District (CityID, DistrictName) VALUES
(52, 'Altınordu'), (52, 'Ünye'), (52, 'Fatsa');
-- 53 Rize
INSERT INTO District (CityID, DistrictName) VALUES
(53, 'Merkez'), (53, 'Çayeli'), (53, 'Ardeşen');
-- 54 Sakarya
INSERT INTO District (CityID, DistrictName) VALUES
(54, 'Adapazarı'), (54, 'Serdivan'), (54, 'Erenler');
-- 55 Samsun
INSERT INTO District (CityID, DistrictName) VALUES
(55, 'İlkadım'), (55, 'Atakum'), (55, 'Bafra');
-- 56 Siirt
INSERT INTO District (CityID, DistrictName) VALUES
(56, 'Merkez'), (56, 'Kurtalan'), (56, 'Pervari');
-- 57 Sinop
INSERT INTO District (CityID, DistrictName) VALUES
(57, 'Merkez'), (57, 'Boyabat'), (57, 'Durağan');
-- 58 Sivas
INSERT INTO District (CityID, DistrictName) VALUES
(58, 'Merkez'), (58, 'Şarkışla'), (58, 'Zara');
-- 59 Tekirdağ
INSERT INTO District (CityID, DistrictName) VALUES
(59, 'Çorlu'), (59, 'Süleymanpaşa'), (59, 'Çerkezköy');
-- 60 Tokat
INSERT INTO District (CityID, DistrictName) VALUES
(60, 'Merkez'), (60, 'Erbaa'), (60, 'Turhal');
-- 61 Trabzon
INSERT INTO District (CityID, DistrictName) VALUES
(61, 'Ortahisar'), (61, 'Akçaabat'), (61, 'Arsin');
-- 62 Tunceli
INSERT INTO District (CityID, DistrictName) VALUES
(62, 'Merkez'), (62, 'Pertek'), (62, 'Çemişgezek');
-- 63 Şanlıurfa
INSERT INTO District (CityID, DistrictName) VALUES
(63, 'Eyyübiye'), (63, 'Haliliye'), (63, 'Siverek');
-- 64 Uşak
INSERT INTO District (CityID, DistrictName) VALUES
(64, 'Merkez'), (64, 'Banaz'), (64, 'Eşme');
-- 65 Van
INSERT INTO District (CityID, DistrictName) VALUES
(65, 'İpekyolu'), (65, 'Edremit'), (65, 'Erciş');
-- 66 Yozgat
INSERT INTO District (CityID, DistrictName) VALUES
(66, 'Merkez'), (66, 'Sorgun'), (66, 'Akdağmadeni');
-- 67 Zonguldak
INSERT INTO District (CityID, DistrictName) VALUES
(67, 'Merkez'), (67, 'Ereğli'), (67, 'Çaycuma');
-- 68 Aksaray
INSERT INTO District (CityID, DistrictName) VALUES
(68, 'Merkez'), (68, 'Eskil'), (68, 'Ortaköy');
-- 69 Bayburt
INSERT INTO District (CityID, DistrictName) VALUES
(69, 'Merkez'), (69, 'Aydıntepe'), (69, 'Demirözü');
-- 70 Karaman
INSERT INTO District (CityID, DistrictName) VALUES
(70, 'Merkez'), (70, 'Ermenek'), (70, 'Kazımkarabekir');
-- 71 Kırıkkale
INSERT INTO District (CityID, DistrictName) VALUES
(71, 'Merkez'), (71, 'Yahşihan'), (71, 'Keskin');
-- 72 Batman
INSERT INTO District (CityID, DistrictName) VALUES
(72, 'Merkez'), (72, 'Kozluk'), (72, 'Beşiri');
-- 73 Şırnak
INSERT INTO District (CityID, DistrictName) VALUES
(73, 'Cizre'), (73, 'Silopi'), (73, 'Merkez');
-- 74 Bartın
INSERT INTO District (CityID, DistrictName) VALUES
(74, 'Merkez'), (74, 'Amasra'), (74, 'Ulus');
-- 75 Ardahan
INSERT INTO District (CityID, DistrictName) VALUES
(75, 'Merkez'), (75, 'Göle'), (75, 'Posof');
-- 76 Iğdır
INSERT INTO District (CityID, DistrictName) VALUES
(76, 'Merkez'), (76, 'Tuzluca'), (76, 'Aralık');
-- 77 Yalova
INSERT INTO District (CityID, DistrictName) VALUES
(77, 'Merkez'), (77, 'Çiftlikköy'), (77, 'Altınova');
-- 78 Karabük
INSERT INTO District (CityID, DistrictName) VALUES
(78, 'Merkez'), (78, 'Safranbolu'), (78, 'Eskipazar');
-- 79 Kilis
INSERT INTO District (CityID, DistrictName) VALUES
(79, 'Merkez'), (79, 'Elbeyli'), (79, 'Musabeyli');
-- 80 Osmaniye
INSERT INTO District (CityID, DistrictName) VALUES
(80, 'Merkez'), (80, 'Kadirli'), (80, 'Düziçi');
-- 81 Düzce
INSERT INTO District (CityID, DistrictName) VALUES
(81, 'Merkez'), (81, 'Akçakoca'), (81, 'Cumayeri');
