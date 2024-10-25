<template>
  <div class="bg-gradient-to-t from-green-400 h-full w-full flex">
    <div class="w-50 h-50 m-auto flex flex-col justify-center">
      <h1 class="text-9xl">404</h1>
      <p class="text-2xl text-center">Page not found</p>
      <button
        class="border border-gray-500 mt-2 rounded-md hover:bg-green-600 hover:text-white py-2"
        @click="$router.go(-1)"
      >
        Back
      </button>
    </div>
  </div>
</template>
