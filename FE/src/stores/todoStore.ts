import { searchTodoList, type SearchTodoListRequest } from '@/api/todoListApi'
import type { Todo } from '@/types/Todo'
import { defineStore } from 'pinia'

interface State {
  search: SearchTodoListRequest
  todos: Todo[]
}

export const useTodoStore = defineStore('todoStore', {
  state: (): State => {
    return {
      search: {
        userId: 0
      },
      todos: []
    }
  },
  actions: {
    setSearch(search: SearchTodoListRequest) {
      this.search = search
    },
    async getTodoListByUserId() {
      const { data } = await searchTodoList(this.search)
      this.todos = data
    }
  }
})
