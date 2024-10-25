export type Province = {
  label: string
  value: number
}

export type District = {
  label: string
  value: number
}

export const province = [
  { label: 'An Giang', value: 89 },
  { label: 'Bà Rịa - Vũng Tàu', value: 77 },
  { label: 'Bạc Liêu', value: 95 },
  { label: 'Bắc Giang', value: 24 },
  { label: 'Bắc Kạn', value: 6 },
  { label: 'Bắc Ninh', value: 27 },
  { label: 'Bến Tre', value: 83 },
  { label: 'Bình Dương', value: 74 },
  { label: 'Bình Định', value: 52 },
  { label: 'Bình Phước', value: 70 },
  { label: 'Bình Thuận', value: 60 },
  { label: 'Cà Mau', value: 96 },
  { label: 'Cao Bằng', value: 4 },
  { label: 'Cần Thơ', value: 92 },
  { label: 'Đà Nẵng', value: 48 },
  { label: 'Đắk Lắk', value: 66 },
  { label: 'Đắk Nông', value: 67 },
  { label: 'Điện Biên', value: 11 },
  { label: 'Đồng Nai', value: 75 },
  { label: 'Đồng Tháp', value: 87 },
  { label: 'Gia Lai', value: 64 },
  { label: 'Hà Giang', value: 2 },
  { label: 'Hà Nam', value: 35 },
  { label: 'Hà Nội', value: 1 },
  { label: 'Hà Tĩnh', value: 42 },
  { label: 'Hải Dương', value: 30 },
  { label: 'Hải Phòng', value: 31 },
  { label: 'Hậu Giang', value: 93 },
  { label: 'Hoà Bình', value: 17 },
  { label: 'Hồ Chí Minh', value: 79 },
  { label: 'Hưng Yên', value: 33 },
  { label: 'Khánh Hòa', value: 56 },
  { label: 'Kiên Giang', value: 91 },
  { label: 'Kon Tum', value: 62 },
  { label: 'Lai Châu', value: 12 },
  { label: 'Lạng Sơn', value: 20 },
  { label: 'Lào Cai', value: 10 },
  { label: 'Lâm Đồng', value: 68 },
  { label: 'Long An', value: 80 },
  { label: 'Nam Định', value: 36 },
  { label: 'Nghệ An', value: 40 },
  { label: 'Ninh Bình', value: 37 },
  { label: 'Ninh Thuận', value: 58 },
  { label: 'Phú Thọ', value: 25 },
  { label: 'Phú Yên', value: 54 },
  { label: 'Quảng Bình', value: 44 },
  { label: 'Quảng Nam', value: 49 },
  { label: 'Quảng Ngãi', value: 51 },
  { label: 'Quảng Ninh', value: 22 },
  { label: 'Quảng Trị', value: 45 },
  { label: 'Sóc Trăng', value: 94 },
  { label: 'Sơn La', value: 14 },
  { label: 'Tây Ninh', value: 72 },
  { label: 'Thái Bình', value: 34 },
  { label: 'Thái Nguyên', value: 19 },
  { label: 'Thanh Hóa', value: 38 },
  { label: 'Thừa Thiên Huế', value: 46 },
  { label: 'Tiền Giang', value: 82 },
  { label: 'Trà Vinh', value: 84 },
  { label: 'Tuyên Quang', value: 8 },
  { label: 'Vĩnh Long', value: 86 },
  { label: 'Vĩnh Phúc', value: 26 },
  { label: 'Yên Bái', value: 15 }
]

export const district = [
  {
    province_id: 1,
    districts: [
      { label: 'Ba Đình', value: 1 },
      { label: 'Hoàn Kiếm', value: 2 },
      { label: 'Tây Hồ', value: 3 },
      { label: 'Long Biên', value: 4 },
      { label: 'Cầu Giấy', value: 5 },
      { label: 'Đống Đa', value: 6 },
      { label: 'Hai Bà Trưng', value: 7 },
      { label: 'Hoàng Mai', value: 8 },
      { label: 'Thanh Xuân', value: 9 },
      { label: 'Sóc Sơn', value: 16 },
      { label: 'Đông Anh', value: 17 },
      { label: 'Gia Lâm', value: 18 },
      { label: 'Nam Từ Liêm', value: 19 },
      { label: 'Thanh Trì', value: 20 },
      { label: 'Bắc Từ Liêm', value: 21 },
      { label: 'Mê Linh', value: 250 },
      { label: 'Hà Đông', value: 268 },
      { label: 'Sơn Tây', value: 269 },
      { label: 'Ba Vì', value: 271 },
      { label: 'Phúc Thọ', value: 272 },
      { label: 'Đan Phượng', value: 273 },
      { label: 'Hoài Đức', value: 274 },
      { label: 'Quốc Oai', value: 275 },
      { label: 'Thạch Thất', value: 276 },
      { label: 'Chương Mỹ', value: 277 },
      { label: 'Thanh Oai', value: 278 },
      { label: 'Thường Tín', value: 279 },
      { label: 'Phú Xuyên', value: 280 },
      { label: 'Ứng Hòa', value: 281 },
      { label: 'Mỹ Đức', value: 282 }
    ]
  },
  {
    province_id: 2,
    districts: [
      { label: 'Hà Giang', value: 24 },
      { label: 'Đồng Văn', value: 26 },
      { label: 'Mèo Vạc', value: 27 },
      { label: 'Yên Minh', value: 28 },
      { label: 'Quản Bạ', value: 29 },
      { label: 'Vị Xuyên', value: 30 },
      { label: 'Bắc Mê', value: 31 },
      { label: 'Hoàng Su Phì', value: 32 },
      { label: 'Xín Mần', value: 33 },
      { label: 'Bắc Quang', value: 34 },
      { label: 'Quang Bình', value: 35 }
    ]
  },
  {
    province_id: 4,
    districts: [
      { label: 'Cao Bằng', value: 40 },
      { label: 'Bảo Lâm', value: 42 },
      { label: 'Bảo Lạc', value: 43 },
      { label: 'Hà Quảng', value: 45 },
      { label: 'Trùng Khánh', value: 47 },
      { label: 'Hạ Lang', value: 48 },
      { label: 'Quảng Hòa', value: 49 },
      { label: 'Hoà An', value: 51 },
      { label: 'Nguyên Bình', value: 52 },
      { label: 'Thạch An', value: 53 }
    ]
  },
  {
    province_id: 6,
    districts: [
      { label: 'Bắc Kạn', value: 58 },
      { label: 'Pác Nặm', value: 60 },
      { label: 'Ba Bể', value: 61 },
      { label: 'Ngân Sơn', value: 62 },
      { label: 'Bạch Thông', value: 63 },
      { label: 'Chợ Đồn', value: 64 },
      { label: 'Chợ Mới', value: 65 },
      { label: 'Na Rì', value: 66 }
    ]
  },
  {
    province_id: 8,
    districts: [
      { label: 'Tuyên Quang', value: 70 },
      { label: 'Lâm Bình', value: 71 },
      { label: 'Na Hang', value: 72 },
      { label: 'Chiêm Hóa', value: 73 },
      { label: 'Hàm Yên', value: 74 },
      { label: 'Yên Sơn', value: 75 },
      { label: 'Sơn Dương', value: 76 }
    ]
  },
  {
    province_id: 10,
    districts: [
      { label: 'Lào Cai', value: 80 },
      { label: 'Bát Xát', value: 82 },
      { label: 'Mường Khương', value: 83 },
      { label: 'Si Ma Cai', value: 84 },
      { label: 'Bắc Hà', value: 85 },
      { label: 'Bảo Thắng', value: 86 },
      { label: 'Bảo Yên', value: 87 },
      { label: 'Sa Pa', value: 88 },
      { label: 'Văn Bàn', value: 89 }
    ]
  },
  {
    province_id: 11,
    districts: [
      { label: 'Điện Biên Phủ', value: 94 },
      { label: 'Mường Lay', value: 95 },
      { label: 'Mường Nhé', value: 96 },
      { label: 'Mường Chà', value: 97 },
      { label: 'Tủa Chùa', value: 98 },
      { label: 'Tuần Giáo', value: 99 },
      { label: 'Điện Biên', value: 100 },
      { label: 'Điện Biên Đông', value: 101 },
      { label: 'Mường Ảng', value: 102 },
      { label: 'Nậm Pồ', value: 103 }
    ]
  },
  {
    province_id: 12,
    districts: [
      { label: 'Lai Châu', value: 105 },
      { label: 'Tam Đường', value: 106 },
      { label: 'Mường Tè', value: 107 },
      { label: 'Sìn Hồ', value: 108 },
      { label: 'Phong Thổ', value: 109 },
      { label: 'Than Uyên', value: 110 },
      { label: 'Tân Uyên', value: 111 },
      { label: 'Nậm Nhùn', value: 112 }
    ]
  },
  {
    province_id: 14,
    districts: [
      { label: 'Sơn La', value: 116 },
      { label: 'Quỳnh Nhai', value: 118 },
      { label: 'Thuận Châu', value: 119 },
      { label: 'Mường La', value: 120 },
      { label: 'Bắc Yên', value: 121 },
      { label: 'Phù Yên', value: 122 },
      { label: 'Mộc Châu', value: 123 },
      { label: 'Yên Châu', value: 124 },
      { label: 'Mai Sơn', value: 125 },
      { label: 'Sông Mã', value: 126 },
      { label: 'Sốp Cộp', value: 127 },
      { label: 'Vân Hồ', value: 128 }
    ]
  },
  {
    province_id: 15,
    districts: [
      { label: 'Yên Bái', value: 132 },
      { label: 'Nghĩa Lộ', value: 133 },
      { label: 'Lục Yên', value: 135 },
      { label: 'Văn Yên', value: 136 },
      { label: 'Mù Căng Chải', value: 137 },
      { label: 'Trấn Yên', value: 138 },
      { label: 'Trạm Tấu', value: 139 },
      { label: 'Văn Chấn', value: 140 },
      { label: 'Yên Bình', value: 141 }
    ]
  },
  {
    province_id: 17,
    districts: [
      { label: 'Hòa Bình', value: 148 },
      { label: 'Đà Bắc', value: 150 },
      { label: 'Lương Sơn', value: 152 },
      { label: 'Kim Bôi', value: 153 },
      { label: 'Cao Phong', value: 154 },
      { label: 'Tân Lạc', value: 155 },
      { label: 'Mai Châu', value: 156 },
      { label: 'Lạc Sơn', value: 157 },
      { label: 'Yên Thủy', value: 158 },
      { label: 'Lạc Thủy', value: 159 }
    ]
  },
  {
    province_id: 19,
    districts: [
      { label: 'Thái Nguyên', value: 164 },
      { label: 'Sông Công', value: 165 },
      { label: 'Định Hóa', value: 167 },
      { label: 'Phú Lương', value: 168 },
      { label: 'Đồng Hỷ', value: 169 },
      { label: 'Võ Nhai', value: 170 },
      { label: 'Đại Từ', value: 171 },
      { label: 'Phổ Yên', value: 172 },
      { label: 'Phú Bình', value: 173 }
    ]
  },
  {
    province_id: 20,
    districts: [
      { label: 'Lạng Sơn', value: 178 },
      { label: 'Tràng Định', value: 180 },
      { label: 'Bình Gia', value: 181 },
      { label: 'Văn Lãng', value: 182 },
      { label: 'Cao Lộc', value: 183 },
      { label: 'Văn Quan', value: 184 },
      { label: 'Bắc Sơn', value: 185 },
      { label: 'Hữu Lũng', value: 186 },
      { label: 'Chi Lăng', value: 187 },
      { label: 'Lộc Bình', value: 188 },
      { label: 'Đình Lập', value: 189 }
    ]
  },
  {
    province_id: 22,
    districts: [
      { label: 'Hạ Long', value: 193 },
      { label: 'Móng Cái', value: 194 },
      { label: 'Cẩm Phả', value: 195 },
      { label: 'Uông Bí', value: 196 },
      { label: 'Bình Liêu', value: 198 },
      { label: 'Tiên Yên', value: 199 },
      { label: 'Đầm Hà', value: 200 },
      { label: 'Hải Hà', value: 201 },
      { label: 'Ba Chẽ', value: 202 },
      { label: 'Vân Đồn', value: 203 },
      { label: 'Đông Triều', value: 205 },
      { label: 'Quảng Yên', value: 206 },
      { label: 'Cô Tô', value: 207 }
    ]
  },
  {
    province_id: 24,
    districts: [
      { label: 'Bắc Giang', value: 213 },
      { label: 'Yên Thế', value: 215 },
      { label: 'Tân Yên', value: 216 },
      { label: 'Lạng Giang', value: 217 },
      { label: 'Lục Nam', value: 218 },
      { label: 'Lục Ngạn', value: 219 },
      { label: 'Sơn Động', value: 220 },
      { label: 'Yên Dũng', value: 221 },
      { label: 'Việt Yên', value: 222 },
      { label: 'Hiệp Hòa', value: 223 }
    ]
  },
  {
    province_id: 25,
    districts: [
      { label: 'Việt Trì', value: 227 },
      { label: 'Phú Thọ', value: 228 },
      { label: 'Đoan Hùng', value: 230 },
      { label: 'Hạ Hoà', value: 231 },
      { label: 'Thanh Ba', value: 232 },
      { label: 'Phù Ninh', value: 233 },
      { label: 'Yên Lập', value: 234 },
      { label: 'Cẩm Khê', value: 235 },
      { label: 'Tam Nông', value: 236 },
      { label: 'Lâm Thao', value: 237 },
      { label: 'Thanh Sơn', value: 238 },
      { label: 'Thanh Thuỷ', value: 239 },
      { label: 'Tân Sơn', value: 240 }
    ]
  },
  {
    province_id: 26,
    districts: [
      { label: 'Vĩnh Yên', value: 243 },
      { label: 'Phúc Yên', value: 244 },
      { label: 'Lập Thạch', value: 246 },
      { label: 'Tam Dương', value: 247 },
      { label: 'Tam Đảo', value: 248 },
      { label: 'Bình Xuyên', value: 249 },
      { label: 'Yên Lạc', value: 251 },
      { label: 'Vĩnh Tường', value: 252 },
      { label: 'Sông Lô', value: 253 }
    ]
  },
  {
    province_id: 27,
    districts: [
      { label: 'Bắc Ninh', value: 256 },
      { label: 'Yên Phong', value: 258 },
      { label: 'Quế Võ', value: 259 },
      { label: 'Tiên Du', value: 260 },
      { label: 'Từ Sơn', value: 261 },
      { label: 'Thuận Thành', value: 262 },
      { label: 'Gia Bình', value: 263 },
      { label: 'Lương Tài', value: 264 }
    ]
  },
  {
    province_id: 30,
    districts: [
      { label: 'Hải Dương', value: 288 },
      { label: 'Chí Linh', value: 290 },
      { label: 'Nam Sách', value: 291 },
      { label: 'Kinh Môn', value: 292 },
      { label: 'Kim Thành', value: 293 },
      { label: 'Thanh Hà', value: 294 },
      { label: 'Cẩm Giàng', value: 295 },
      { label: 'Bình Giang', value: 296 },
      { label: 'Gia Lộc', value: 297 },
      { label: 'Tứ Kỳ', value: 298 },
      { label: 'Ninh Giang', value: 299 },
      { label: 'Thanh Miện', value: 300 }
    ]
  },
  {
    province_id: 31,
    districts: [
      { label: 'Hồng Bàng', value: 303 },
      { label: 'Ngô Quyền', value: 304 },
      { label: 'Lê Chân', value: 305 },
      { label: 'Hải An', value: 306 },
      { label: 'Kiến An', value: 307 },
      { label: 'Đồ Sơn', value: 308 },
      { label: 'Dương Kinh', value: 309 },
      { label: 'Thuỷ Nguyên', value: 311 },
      { label: 'An Dương', value: 312 },
      { label: 'An Lão', value: 313 },
      { label: 'Kiến Thuỵ', value: 314 },
      { label: 'Tiên Lãng', value: 315 },
      { label: 'Vĩnh Bảo', value: 316 },
      { label: 'Cát Hải', value: 317 },
      { label: 'Bạch Long Vĩ', value: 318 }
    ]
  },
  {
    province_id: 33,
    districts: [
      { label: 'Hưng Yên', value: 323 },
      { label: 'Văn Lâm', value: 325 },
      { label: 'Văn Giang', value: 326 },
      { label: 'Yên Mỹ', value: 327 },
      { label: 'Mỹ Hào', value: 328 },
      { label: 'Ân Thi', value: 329 },
      { label: 'Khoái Châu', value: 330 },
      { label: 'Kim Động', value: 331 },
      { label: 'Tiên Lữ', value: 332 },
      { label: 'Phù Cừ', value: 333 }
    ]
  },
  {
    province_id: 34,
    districts: [
      { label: 'Thái Bình', value: 336 },
      { label: 'Quỳnh Phụ', value: 338 },
      { label: 'Hưng Hà', value: 339 },
      { label: 'Đông Hưng', value: 340 },
      { label: 'Thái Thụy', value: 341 },
      { label: 'Tiền Hải', value: 342 },
      { label: 'Kiến Xương', value: 343 },
      { label: 'Vũ Thư', value: 344 }
    ]
  },
  {
    province_id: 35,
    districts: [
      { label: 'Phủ Lý', value: 347 },
      { label: 'Duy Tiên', value: 349 },
      { label: 'Kim Bảng', value: 350 },
      { label: 'Thanh Liêm', value: 351 },
      { label: 'Bình Lục', value: 352 },
      { label: 'Lý Nhân', value: 353 }
    ]
  },
  {
    province_id: 36,
    districts: [
      { label: 'Nam Định', value: 356 },
      { label: 'Mỹ Lộc', value: 358 },
      { label: 'Vụ Bản', value: 359 },
      { label: 'Ý Yên', value: 360 },
      { label: 'Nghĩa Hưng', value: 361 },
      { label: 'Nam Trực', value: 362 },
      { label: 'Trực Ninh', value: 363 },
      { label: 'Xuân Trường', value: 364 },
      { label: 'Giao Thủy', value: 365 },
      { label: 'Hải Hậu', value: 366 }
    ]
  },
  {
    province_id: 37,
    districts: [
      { label: 'Ninh Bình', value: 369 },
      { label: 'Tam Điệp', value: 370 },
      { label: 'Nho Quan', value: 372 },
      { label: 'Gia Viễn', value: 373 },
      { label: 'Hoa Lư', value: 374 },
      { label: 'Yên Khánh', value: 375 },
      { label: 'Kim Sơn', value: 376 },
      { label: 'Yên Mô', value: 377 }
    ]
  },
  {
    province_id: 38,
    districts: [
      { label: 'Thanh Hóa', value: 380 },
      { label: 'Bỉm Sơn', value: 381 },
      { label: 'Sầm Sơn', value: 382 },
      { label: 'Mường Lát', value: 384 },
      { label: 'Quan Hóa', value: 385 },
      { label: 'Bá Thước', value: 386 },
      { label: 'Quan Sơn', value: 387 },
      { label: 'Lang Chánh', value: 388 },
      { label: 'Ngọc Lặc', value: 389 },
      { label: 'Cẩm Thủy', value: 390 },
      { label: 'Thạch Thành', value: 391 },
      { label: 'Hà Trung', value: 392 },
      { label: 'Vĩnh Lộc', value: 393 },
      { label: 'Yên Định', value: 394 },
      { label: 'Thọ Xuân', value: 395 },
      { label: 'Thường Xuân', value: 396 },
      { label: 'Triệu Sơn', value: 397 },
      { label: 'Thiệu Hóa', value: 398 },
      { label: 'Hoằng Hóa', value: 399 },
      { label: 'Hậu Lộc', value: 400 },
      { label: 'Nga Sơn', value: 401 },
      { label: 'Như Xuân', value: 402 },
      { label: 'Như Thanh', value: 403 },
      { label: 'Nông Cống', value: 404 },
      { label: 'Đông Sơn', value: 405 },
      { label: 'Quảng Xương', value: 406 },
      { label: 'Nghi Sơn', value: 407 }
    ]
  },
  {
    province_id: 40,
    districts: [
      { label: 'Vinh', value: 412 },
      { label: 'Cửa Lò', value: 413 },
      { label: 'Thái Hoà', value: 414 },
      { label: 'Quế Phong', value: 415 },
      { label: 'Quỳ Châu', value: 416 },
      { label: 'Kỳ Sơn', value: 417 },
      { label: 'Tương Dương', value: 418 },
      { label: 'Nghĩa Đàn', value: 419 },
      { label: 'Quỳ Hợp', value: 420 },
      { label: 'Quỳnh Lưu', value: 421 },
      { label: 'Con Cuông', value: 422 },
      { label: 'Tân Kỳ', value: 423 },
      { label: 'Anh Sơn', value: 424 },
      { label: 'Diễn Châu', value: 425 },
      { label: 'Yên Thành', value: 426 },
      { label: 'Đô Lương', value: 427 },
      { label: 'Thanh Chương', value: 428 },
      { label: 'Nghi Lộc', value: 429 },
      { label: 'Nam Đàn', value: 430 },
      { label: 'Hưng Nguyên', value: 431 },
      { label: 'Hoàng Mai', value: 432 }
    ]
  },
  {
    province_id: 42,
    districts: [
      { label: 'Hà Tĩnh', value: 436 },
      { label: 'Hồng Lĩnh', value: 437 },
      { label: 'Hương Sơn', value: 439 },
      { label: 'Đức Thọ', value: 440 },
      { label: 'Vũ Quang', value: 441 },
      { label: 'Nghi Xuân', value: 442 },
      { label: 'Can Lộc', value: 443 },
      { label: 'Hương Khê', value: 444 },
      { label: 'Thạch Hà', value: 445 },
      { label: 'Cẩm Xuyên', value: 446 },
      { label: 'Kỳ Anh', value: 447 },
      { label: 'Lộc Hà', value: 448 },
      { label: 'Kỳ Anh', value: 449 }
    ]
  },
  {
    province_id: 44,
    districts: [
      { label: 'Đồng Hới', value: 450 },
      { label: 'Minh Hóa', value: 452 },
      { label: 'Tuyên Hóa', value: 453 },
      { label: 'Quảng Trạch', value: 454 },
      { label: 'Bố Trạch', value: 455 },
      { label: 'Quảng Ninh', value: 456 },
      { label: 'Lệ Thủy', value: 457 }
    ]
  },
  {
    province_id: 45,
    districts: [
      { label: 'Đông Hà', value: 461 },
      { label: 'Quảng Trị', value: 462 },
      { label: 'Vĩnh Linh', value: 464 },
      { label: 'Hướng Hóa', value: 465 },
      { label: 'Gio Linh', value: 466 },
      { label: 'Đa Krông', value: 467 },
      { label: 'Cam Lộ', value: 468 },
      { label: 'Triệu Phong', value: 469 },
      { label: 'Hải Lăng', value: 470 },
      { label: 'Cồn Cỏ', value: 471 }
    ]
  },
  {
    province_id: 46,
    districts: [
      { label: 'Huế', value: 474 },
      { label: 'Phong Điền', value: 476 },
      { label: 'Quảng Điền', value: 477 },
      { label: 'Phú Vang', value: 478 },
      { label: 'Hương Thủy', value: 479 },
      { label: 'Hương Trà', value: 480 },
      { label: 'A Lưới', value: 481 },
      { label: 'Phú Lộc', value: 482 },
      { label: 'Nam Đông', value: 483 }
    ]
  },
  {
    province_id: 48,
    districts: [
      { label: 'Liên Chiểu', value: 490 },
      { label: 'Thanh Khê', value: 491 },
      { label: 'Hải Châu', value: 492 },
      { label: 'Sơn Trà', value: 493 },
      { label: 'Ngũ Hành Sơn', value: 494 },
      { label: 'Cẩm Lệ', value: 495 }
    ]
  },
  {
    province_id: 48,
    districts: [
      { label: 'Liên Chiểu', value: 490 },
      { label: 'Thanh Khê', value: 491 },
      { label: 'Hải Châu', value: 492 },
      { label: 'Sơn Trà', value: 493 },
      { label: 'Ngũ Hành Sơn', value: 494 },
      { label: 'Cẩm Lệ', value: 495 },
      { label: 'Hòa Vang', value: 497 },
      { label: 'Hoàng Sa', value: 498 }
    ]
  },
  {
    province_id: 49,
    districts: [
      { label: 'Tam Kỳ', value: 502 },
      { label: 'Hội An', value: 503 },
      { label: 'Tây Giang', value: 504 },
      { label: 'Đông Giang', value: 505 },
      { label: 'Đại Lộc', value: 506 },
      { label: 'Điện Bàn', value: 507 },
      { label: 'Duy Xuyên', value: 508 },
      { label: 'Quế Sơn', value: 509 },
      { label: 'Nam Giang', value: 510 },
      { label: 'Phước Sơn', value: 511 },
      { label: 'Hiệp Đức', value: 512 },
      { label: 'Thăng Bình', value: 513 },
      { label: 'Tiên Phước', value: 514 },
      { label: 'Bắc Trà My', value: 515 },
      { label: 'Nam Trà My', value: 516 },
      { label: 'Núi Thành', value: 517 },
      { label: 'Phú Ninh', value: 518 },
      { label: 'Nông Sơn', value: 519 }
    ]
  },
  {
    province_id: 51,
    districts: [
      { label: 'Quảng Ngãi', value: 522 },
      { label: 'Bình Sơn', value: 524 },
      { label: 'Trà Bồng', value: 525 },
      { label: 'Sơn Tịnh', value: 527 },
      { label: 'Tư Nghĩa', value: 528 },
      { label: 'Sơn Hà', value: 529 },
      { label: 'Sơn Tây', value: 530 },
      { label: 'Minh Long', value: 531 },
      { label: 'Nghĩa Hành', value: 532 },
      { label: 'Mộ Đức', value: 533 },
      { label: 'Đức Phổ', value: 534 },
      { label: 'Ba Tơ', value: 535 },
      { label: 'Lý Sơn', value: 536 }
    ]
  },
  {
    province_id: 52,
    districts: [
      { label: 'Quy Nhơn', value: 540 },
      { label: 'An Lão', value: 542 },
      { label: 'Hoài Nhơn', value: 543 },
      { label: 'Hoài Ân', value: 544 },
      { label: 'Phù Mỹ', value: 545 },
      { label: 'Vĩnh Thạnh', value: 546 },
      { label: 'Tây Sơn', value: 547 },
      { label: 'Phù Cát', value: 548 },
      { label: 'An Nhơn', value: 549 },
      { label: 'Tuy Phước', value: 550 },
      { label: 'Vân Canh', value: 551 }
    ]
  },
  {
    province_id: 54,
    districts: [
      { label: 'Tuy Hoà', value: 555 },
      { label: 'Sông Cầu', value: 557 },
      { label: 'Đồng Xuân', value: 558 },
      { label: 'Tuy An', value: 559 },
      { label: 'Sơn Hòa', value: 560 },
      { label: 'Sông Hinh', value: 561 },
      { label: 'Tây Hoà', value: 562 },
      { label: 'Phú Hoà', value: 563 },
      { label: 'Đông Hòa', value: 564 }
    ]
  },
  {
    province_id: 56,
    districts: [
      { label: 'Nha Trang', value: 568 },
      { label: 'Cam Ranh', value: 569 },
      { label: 'Cam Lâm', value: 570 },
      { label: 'Vạn Ninh', value: 571 },
      { label: 'Ninh Hòa', value: 572 },
      { label: 'Khánh Vĩnh', value: 573 },
      { label: 'Diên Khánh', value: 574 },
      { label: 'Khánh Sơn', value: 575 },
      { label: 'Trường Sa', value: 576 }
    ]
  },
  {
    province_id: 58,
    districts: [
      { label: 'Phan Rang-Tháp Chàm', value: 582 },
      { label: 'Bác Ái', value: 584 },
      { label: 'Ninh Sơn', value: 585 },
      { label: 'Ninh Hải', value: 586 },
      { label: 'Ninh Phước', value: 587 },
      { label: 'Thuận Bắc', value: 588 },
      { label: 'Thuận Nam', value: 589 }
    ]
  },
  {
    province_id: 60,
    districts: [
      { label: 'Phan Thiết', value: 593 },
      { label: 'La Gi', value: 594 },
      { label: 'Tuy Phong', value: 595 },
      { label: 'Bắc Bình', value: 596 },
      { label: 'Hàm Thuận Bắc', value: 597 },
      { label: 'Hàm Thuận Nam', value: 598 },
      { label: 'Tánh Linh', value: 599 },
      { label: 'Đức Linh', value: 600 },
      { label: 'Hàm Tân', value: 601 },
      { label: 'Phú Quí', value: 602 }
    ]
  },
  {
    province_id: 62,
    districts: [
      { label: 'Kon Tum', value: 608 },
      { label: 'Đắk Glei', value: 610 },
      { label: 'Ngọc Hồi', value: 611 },
      { label: 'Đắk Tô', value: 612 },
      { label: 'Kon Plông', value: 613 },
      { label: 'Kon Rẫy', value: 614 },
      { label: 'Đắk Hà', value: 615 },
      { label: 'Sa Thầy', value: 616 },
      { label: 'Tu Mơ Rông', value: 617 },
      { label: "Ia H' Drai", value: 618 }
    ]
  },
  {
    province_id: 64,
    districts: [
      { label: 'Pleiku', value: 622 },
      { label: 'An Khê', value: 623 },
      { label: 'Ayun Pa', value: 624 },
      { label: 'KBang', value: 625 },
      { label: 'Đăk Đoa', value: 626 },
      { label: 'Chư Păh', value: 627 },
      { label: 'Ia Grai', value: 628 },
      { label: 'Mang Yang', value: 629 },
      { label: 'Kông Chro', value: 630 },
      { label: 'Chư Sê', value: 631 },
      { label: 'Chư Prông', value: 632 },
      { label: 'Đăk Pơ', value: 633 },
      { label: 'Đăk Sơr', value: 634 },
      { label: 'Đăk Tô', value: 635 }
    ]
  },
  {
    province_id: 66,
    districts: [
      { label: 'Gia Nghĩa', value: 638 },
      { label: 'Cư Jút', value: 639 },
      { label: 'Đắk Mil', value: 640 },
      { label: 'Đắk Glong', value: 641 },
      { label: "Đắk R'Lấp", value: 642 },
      { label: 'Đắk Song', value: 643 },
      { label: 'Krông Nô', value: 644 },
      { label: 'Tuy Đức', value: 645 }
    ]
  },
  {
    province_id: 67,
    districts: [
      { label: 'Đắk Lắk', value: 649 },
      { label: 'Buôn Ma Thuột', value: 650 },
      { label: "Ea H'leo", value: 651 },
      { label: 'Krông Ana', value: 652 },
      { label: 'Krông Búk', value: 653 },
      { label: 'Krông Năng', value: 654 },
      { label: 'Krông Bông', value: 655 },
      { label: "M'Drắk", value: 656 },
      { label: "Cư M'gar", value: 657 },
      { label: 'Buôn Đôn', value: 658 },
      { label: 'Ea Súp', value: 659 },
      { label: 'Ea Kar', value: 660 },
      { label: 'Lắk', value: 661 },
      { label: 'Yok Đôn', value: 662 }
    ]
  },
  {
    province_id: 68,
    districts: [
      { label: 'Bảo Lộc', value: 665 },
      { label: 'Đà Lạt', value: 666 },
      { label: 'Di Linh', value: 667 },
      { label: 'Đơn Dương', value: 668 },
      { label: 'Lạc Dương', value: 669 },
      { label: 'Lâm Hà', value: 670 },
      { label: 'Đạ Huoai', value: 671 },
      { label: 'Đạ Tẻh', value: 672 },
      { label: 'Cát Tiên', value: 673 }
    ]
  },
  {
    province_id: 75,
    districts: [
      { label: 'Biên Hòa', value: 731 },
      { label: 'Long Khánh', value: 732 },
      { label: 'Tân Phú', value: 734 },
      { label: 'Vĩnh Cửu', value: 735 },
      { label: 'Định Quán', value: 736 },
      { label: 'Trảng Bom', value: 737 },
      { label: 'Thống Nhất', value: 738 },
      { label: 'Cẩm Mỹ', value: 739 },
      { label: 'Long Thành', value: 740 },
      { label: 'Xuân Lộc', value: 741 },
      { label: 'Nhơn Trạch', value: 742 }
    ]
  },
  {
    province_id: 77,
    districts: [
      { label: 'Vũng Tàu', value: 747 },
      { label: 'Bà Rịa', value: 748 },
      { label: 'Châu Đức', value: 750 },
      { label: 'Xuyên Mộc', value: 751 },
      { label: 'Long Điền', value: 752 },
      { label: 'Đất Đỏ', value: 753 },
      { label: 'Phú Mỹ', value: 754 },
      { label: 'Côn Đảo', value: 755 }
    ]
  },
  {
    province_id: 79,
    districts: [
      { label: '1', value: 760 },
      { label: '12', value: 761 },
      { label: 'Gò Vấp', value: 764 },
      { label: 'Bình Thạnh', value: 765 },
      { label: 'Tân Bình', value: 766 },
      { label: 'Tân Phú', value: 767 },
      { label: 'Phú Nhuận', value: 768 },
      { label: 'Thủ Đức', value: 769 },
      { label: '3', value: 770 },
      { label: '10', value: 771 },
      { label: '11', value: 772 },
      { label: '4', value: 773 },
      { label: '5', value: 774 },
      { label: '6', value: 775 },
      { label: '8', value: 776 },
      { label: 'Bình Tân', value: 777 },
      { label: '7', value: 778 },
      { label: 'Củ Chi', value: 783 },
      { label: 'Hóc Môn', value: 784 },
      { label: 'Bình Chánh', value: 785 },
      { label: 'Nhà Bè', value: 786 },
      { label: 'Cần Giờ', value: 787 }
    ]
  },
  {
    province_id: 80,
    districts: [
      { label: 'Tân An', value: 794 },
      { label: 'Kiến Tường', value: 795 },
      { label: 'Tân Hưng', value: 796 },
      { label: 'Vĩnh Hưng', value: 797 },
      { label: 'Mộc Hóa', value: 798 },
      { label: 'Tân Thạnh', value: 799 },
      { label: 'Thạnh Hóa', value: 800 },
      { label: 'Đức Huệ', value: 801 },
      { label: 'Đức Hòa', value: 802 },
      { label: 'Bến Lức', value: 803 },
      { label: 'Thủ Thừa', value: 804 },
      { label: 'Tân Trụ', value: 805 },
      { label: 'Cần Đước', value: 806 },
      { label: 'Cần Giuộc', value: 807 },
      { label: 'Châu Thành', value: 808 }
    ]
  },
  {
    province_id: 82,
    districts: [
      { label: 'Mỹ Tho', value: 815 },
      { label: 'Gò Công', value: 816 },
      { label: 'Cai Lậy', value: 817 },
      { label: 'Tân Phước', value: 818 },
      { label: 'Cái Bè', value: 819 },
      { label: 'Châu Thành', value: 821 },
      { label: 'Chợ Gạo', value: 822 },
      { label: 'Gò Công Tây', value: 823 },
      { label: 'Gò Công Đông', value: 824 },
      { label: 'Tân Phú Đông', value: 825 }
    ]
  },
  {
    province_id: 83,
    districts: [
      { label: 'Bến Tre', value: 829 },
      { label: 'Châu Thành', value: 831 },
      { label: 'Chợ Lách', value: 832 },
      { label: 'Mỏ Cày Nam', value: 833 },
      { label: 'Giồng Trôm', value: 834 },
      { label: 'Bình Đại', value: 835 },
      { label: 'Ba Tri', value: 836 },
      { label: 'Thạnh Phú', value: 837 },
      { label: 'Mỏ Cày Bắc', value: 838 }
    ]
  },
  {
    province_id: 84,
    districts: [
      { label: 'Trà Vinh', value: 842 },
      { label: 'Càng Long', value: 844 },
      { label: 'Cầu Kè', value: 845 },
      { label: 'Tiểu Cần', value: 846 },
      { label: 'Châu Thành', value: 847 },
      { label: 'Cầu Ngang', value: 848 },
      { label: 'Trà Cú', value: 849 },
      { label: 'Duyên Hải', value: 850 },
      { label: 'Duyên Hải', value: 851 }
    ]
  },
  {
    province_id: 86,
    districts: [
      { label: 'Vĩnh Long', value: 855 },
      { label: 'Long Hồ', value: 857 },
      { label: 'Mang Thít', value: 858 },
      { label: 'Vũng Liêm', value: 859 },
      { label: 'Tam Bình', value: 860 },
      { label: 'Bình Minh', value: 861 },
      { label: 'Trà Ôn', value: 862 },
      { label: 'Bình Tân', value: 863 }
    ]
  },
  {
    province_id: 87,
    districts: [
      { label: 'Cao Lãnh', value: 866 },
      { label: 'Sa Đéc', value: 867 },
      { label: 'Hồng Ngự', value: 868 },
      { label: 'Tân Hồng', value: 869 },
      { label: 'Tam Nông', value: 871 },
      { label: 'Tháp Mười', value: 872 },
      { label: 'Thanh Bình', value: 874 },
      { label: 'Lấp Vò', value: 875 },
      { label: 'Lai Vung', value: 876 },
      { label: 'Châu Thành', value: 877 }
    ]
  },
  {
    province_id: 75,
    districts: [
      { label: 'Biên Hòa', value: 731 },
      { label: 'Long Khánh', value: 732 },
      { label: 'Tân Phú', value: 734 },
      { label: 'Vĩnh Cửu', value: 735 },
      { label: 'Định Quán', value: 736 },
      { label: 'Trảng Bom', value: 737 },
      { label: 'Thống Nhất', value: 738 },
      { label: 'Cẩm Mỹ', value: 739 },
      { label: 'Long Thành', value: 740 },
      { label: 'Xuân Lộc', value: 741 },
      { label: 'Nhơn Trạch', value: 742 }
    ]
  },
  {
    province_id: 77,
    districts: [
      { label: 'Vũng Tàu', value: 747 },
      { label: 'Bà Rịa', value: 748 },
      { label: 'Châu Đức', value: 750 },
      { label: 'Xuyên Mộc', value: 751 },
      { label: 'Long Điền', value: 752 },
      { label: 'Đất Đỏ', value: 753 },
      { label: 'Phú Mỹ', value: 754 },
      { label: 'Côn Đảo', value: 755 }
    ]
  },
  {
    province_id: 79,
    districts: [
      { label: '1', value: 760 },
      { label: '12', value: 761 },
      { label: 'Gò Vấp', value: 764 },
      { label: 'Bình Thạnh', value: 765 },
      { label: 'Tân Bình', value: 766 },
      { label: 'Tân Phú', value: 767 },
      { label: 'Phú Nhuận', value: 768 },
      { label: 'Thủ Đức', value: 769 },
      { label: '3', value: 770 },
      { label: '10', value: 771 },
      { label: '11', value: 772 },
      { label: '4', value: 773 },
      { label: '5', value: 774 },
      { label: '6', value: 775 },
      { label: '8', value: 776 },
      { label: 'Bình Tân', value: 777 },
      { label: '7', value: 778 },
      { label: 'Củ Chi', value: 783 },
      { label: 'Hóc Môn', value: 784 },
      { label: 'Bình Chánh', value: 785 },
      { label: 'Nhà Bè', value: 786 },
      { label: 'Cần Giờ', value: 787 }
    ]
  },
  {
    province_id: 80,
    districts: [
      { label: 'Tân An', value: 794 },
      { label: 'Kiến Tường', value: 795 },
      { label: 'Tân Hưng', value: 796 },
      { label: 'Vĩnh Hưng', value: 797 },
      { label: 'Mộc Hóa', value: 798 },
      { label: 'Tân Thạnh', value: 799 },
      { label: 'Thạnh Hóa', value: 800 },
      { label: 'Đức Huệ', value: 801 },
      { label: 'Đức Hòa', value: 802 },
      { label: 'Bến Lức', value: 803 },
      { label: 'Thủ Thừa', value: 804 },
      { label: 'Tân Trụ', value: 805 },
      { label: 'Cần Đước', value: 806 },
      { label: 'Cần Giuộc', value: 807 },
      { label: 'Châu Thành', value: 808 }
    ]
  },
  {
    province_id: 82,
    districts: [
      { label: 'Mỹ Tho', value: 815 },
      { label: 'Gò Công', value: 816 },
      { label: 'Cai Lậy', value: 817 },
      { label: 'Tân Phước', value: 818 },
      { label: 'Cái Bè', value: 819 },
      { label: 'Châu Thành', value: 821 },
      { label: 'Chợ Gạo', value: 822 },
      { label: 'Gò Công Tây', value: 823 },
      { label: 'Gò Công Đông', value: 824 },
      { label: 'Tân Phú Đông', value: 825 }
    ]
  },
  {
    province_id: 83,
    districts: [
      { label: 'Bến Tre', value: 829 },
      { label: 'Châu Thành', value: 831 },
      { label: 'Chợ Lách', value: 832 },
      { label: 'Mỏ Cày Nam', value: 833 },
      { label: 'Giồng Trôm', value: 834 },
      { label: 'Bình Đại', value: 835 },
      { label: 'Ba Tri', value: 836 },
      { label: 'Thạnh Phú', value: 837 },
      { label: 'Mỏ Cày Bắc', value: 838 }
    ]
  },
  {
    province_id: 84,
    districts: [
      { label: 'Trà Vinh', value: 842 },
      { label: 'Càng Long', value: 844 },
      { label: 'Cầu Kè', value: 845 },
      { label: 'Tiểu Cần', value: 846 },
      { label: 'Châu Thành', value: 847 },
      { label: 'Cầu Ngang', value: 848 },
      { label: 'Trà Cú', value: 849 },
      { label: 'Duyên Hải', value: 850 },
      { label: 'Duyên Hải', value: 851 }
    ]
  },
  {
    province_id: 86,
    districts: [
      { label: 'Vĩnh Long', value: 855 },
      { label: 'Long Hồ', value: 857 },
      { label: 'Mang Thít', value: 858 },
      { label: 'Vũng Liêm', value: 859 },
      { label: 'Tam Bình', value: 860 },
      { label: 'Bình Minh', value: 861 },
      { label: 'Trà Ôn', value: 862 },
      { label: 'Bình Tân', value: 863 }
    ]
  },
  {
    province_id: 87,
    districts: [
      { label: 'Cao Lãnh', value: 866 },
      { label: 'Sa Đéc', value: 867 },
      { label: 'Hồng Ngự', value: 868 },
      { label: 'Tân Hồng', value: 869 },
      { label: 'Tam Nông', value: 871 },
      { label: 'Tháp Mười', value: 872 },
      { label: 'Thanh Bình', value: 874 },
      { label: 'Lấp Vò', value: 875 },
      { label: 'Lai Vung', value: 876 },
      { label: 'Châu Thành', value: 877 }
    ]
  },
  {
    province_id: 89,
    districts: [
      { label: 'Long Xuyên', value: 883 },
      { label: 'Châu Đốc', value: 884 },
      { label: 'An Phú', value: 886 },
      { label: 'Tân Châu', value: 887 },
      { label: 'Phú Tân', value: 888 },
      { label: 'Châu Phú', value: 889 },
      { label: 'Tịnh Biên', value: 890 },
      { label: 'Tri Tôn', value: 891 },
      { label: 'Châu Thành', value: 892 },
      { label: 'Chợ Mới', value: 893 },
      { label: 'Thoại Sơn', value: 894 }
    ]
  },
  {
    province_id: 91,
    districts: [
      { label: 'Rạch Giá', value: 899 },
      {
        label: 'Hà Tiên',
        value: 900
      },
      {
        label: 'Kiên Lương',
        value: 902
      },
      {
        label: 'Hòn Đất',
        value: 903
      },
      {
        label: 'Tân Hiệp',
        value: 904
      },
      {
        label: 'Châu Thành',
        value: 905
      },
      {
        label: 'Giồng Riềng',
        value: 906
      },
      {
        label: 'Gò Quao',
        value: 907
      },
      {
        label: 'An Biên',
        value: 908
      },
      {
        label: 'An Minh',
        value: 909
      },
      {
        label: 'Vĩnh Thuận',
        value: 910
      },
      {
        label: 'Phú Quốc',
        value: 911
      },
      {
        label: 'Kiên Hải',
        value: 912
      },
      {
        label: 'U Minh Thượng',
        value: 913
      },
      {
        label: 'Giang Thành',
        value: 914
      }
    ]
  },
  {
    province_id: 92,
    districts: [
      {
        label: 'Ninh Kiều',
        value: 916
      },
      {
        label: 'Ô Môn',
        value: 917
      },
      {
        label: 'Bình Thuỷ',
        value: 918
      },
      {
        label: 'Cái Răng',
        value: 919
      },
      {
        label: 'Thốt Nốt',
        value: 923
      },
      {
        label: 'Vĩnh Thạnh',
        value: 924
      },
      {
        label: 'Cờ Đỏ',
        value: 925
      },
      {
        label: 'Phong Điền',
        value: 926
      },
      {
        label: 'Thới Lai',
        value: 927
      }
    ]
  },
  {
    province_id: 93,
    districts: [
      {
        label: 'Vị Thanh',
        value: 930
      },
      {
        label: 'Ngã Bảy',
        value: 931
      },
      {
        label: 'Châu Thành A',
        value: 932
      },
      {
        label: 'Châu Thành',
        value: 933
      },
      {
        label: 'Phụng Hiệp',
        value: 934
      },
      {
        label: 'Vị Thuỷ',
        value: 935
      },
      {
        label: 'Long Mỹ',
        value: 936
      },
      {
        label: 'Long Mỹ',
        value: 937
      }
    ]
  },
  {
    province_id: 94,
    districts: [
      {
        label: 'Sóc Trăng',
        value: 941
      },
      {
        label: 'Châu Thành',
        value: 942
      },
      {
        label: 'Kế Sách',
        value: 943
      },
      {
        label: 'Mỹ Tú',
        value: 944
      },
      {
        label: 'Cù Lao Dung',
        value: 945
      },
      {
        label: 'Long Phú',
        value: 946
      },
      {
        label: 'Mỹ Xuyên',
        value: 947
      },
      {
        label: 'Ngã Năm',
        value: 948
      },
      {
        label: 'Thạnh Trị',
        value: 949
      },
      {
        label: 'Vĩnh Châu',
        value: 950
      },
      {
        label: 'Trần Đề',
        value: 951
      }
    ]
  },
  {
    province_id: 95,
    districts: [
      {
        label: 'Bạc Liêu',
        value: 954
      },
      {
        label: 'Hồng Dân',
        value: 956
      },
      {
        label: 'Phước Long',
        value: 957
      },
      {
        label: 'Vĩnh Lợi',
        value: 958
      },
      {
        label: 'Giá Rai',
        value: 959
      },
      {
        label: 'Đông Hải',
        value: 960
      },
      {
        label: 'Hoà Bình',
        value: 961
      }
    ]
  },
  {
    province_id: 96,
    districts: [
      {
        label: 'Cà Mau',
        value: 964
      },
      {
        label: 'U Minh',
        value: 966
      },
      {
        label: 'Thới Bình',
        value: 967
      },
      {
        label: 'Trần Văn Thời',
        value: 968
      },
      {
        label: 'Cái Nước',
        value: 969
      },
      {
        label: 'Đầm Dơi',
        value: 970
      },
      {
        label: 'Năm Căn',
        value: 971
      },
      {
        label: 'Phú Tân',
        value: 972
      },
      {
        label: 'Ngọc Hiển',
        value: 973
      }
    ]
  }
]
