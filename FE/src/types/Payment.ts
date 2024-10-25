export type Payment = {
  id: number
  type: string
  amount: number
  status: string
  description: string
  userId: number
  userName: string
  reciverId: number //receiverId
  reciverName: string //receiverName
  bookingId: number
  paymentLink: string
  createdAt: string
  lastUpdatedAt: string
}
