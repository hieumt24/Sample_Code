import { defineRule } from 'vee-validate'
// defineRule('minLength', (value: string, limit: number) => {
//   // The field is empty so it should pass
//   if (!value?.length) {
//     return true;
//   }
//   if (value.length < limit) {
//     return `This field must be at least ${limit} characters`;
//   }
//   return true;
// });

defineRule('required', (value: string) => {
  if (!value?.length) {
    return 'Thsssis field is required'
  }
  return true
})
