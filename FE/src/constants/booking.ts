export enum BookingStatus {
  WAITING = 'WAITING',
  ACCEPTED = 'ACCEPTED',
  CANCELED = 'CANCELED',
  REJECTED = 'REJECTED'
}

export enum BookingIconAndColor {
  ACCEPTED = 'pi pi-check-circle text-green-500',
  REJECTED = 'pi pi-ban text-red-500',
  WAITING = 'pi pi-wave-pulse text-violet-500',
  CANCELED = 'pi pi-undo text-gray-400',
  undefinded = 'pi pi-undo text-red-400'
}
