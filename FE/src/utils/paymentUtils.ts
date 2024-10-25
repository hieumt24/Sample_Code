import {
  InvalidFormatError,
  InvalidNumberError,
  NotEnoughUnitError,
  ReadingConfig,
  doReadNumber
} from 'read-vietnamese-number'

export const getVietnameseMoney = (money: string | number): string => {
  let resultText = ''

  const config = new ReadingConfig()
  config.unit = ['đồng']
  try {
    const result = doReadNumber(config, money + '')
    resultText = result.charAt(0).toUpperCase() + result.slice(1)
    return resultText
  } catch (err) {
    if (err instanceof InvalidFormatError) {
      return 'Định dạng không hợp lệ'
    } else if (err instanceof InvalidNumberError) {
      return 'Số không hợp lệ'
    } else if (err instanceof NotEnoughUnitError) {
      return 'Không đủ đơn vị đọc số'
    }
  }
  return ''
}
