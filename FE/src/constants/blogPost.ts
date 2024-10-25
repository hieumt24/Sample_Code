export enum CategoryPost {
  ANNOUCMENT = 'ANNOUCMENT',
  TOURNAMENT = 'TOURNAMENT',
  PROMOTION = 'PROMOTION'
}

export const categoryOptions = [
  {
    name: 'Thông báo',
    code: CategoryPost.ANNOUCMENT
  },
  {
    name: 'Giải đấu',
    code: CategoryPost.TOURNAMENT
  },
  {
    name: 'Quảng cáo',
    code: CategoryPost.PROMOTION
  }
]
