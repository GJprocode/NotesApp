<template>
  <div class="container mx-auto p-4">
    <div class="flex justify-between items-center mb-6">
      <h1 class="text-3xl font-bold">Your Notes</h1>
      <button @click="logout" class="btn btn-secondary">Logout</button>
    </div>
    <div class="mb-4">
      <input
        v-model="searchQuery"
        type="text"
        class="input"
        placeholder="Search notes by title or content..."
      />
    </div>
    <form @submit.prevent="handleCreateNote" class="mb-6">
      <h2 class="text-2xl font-bold mb-4">Create Note</h2>
      <div class="mb-4">
        <label class="block mb-2 font-medium">Title</label>
        <input v-model="newNote.title" type="text" class="input" required />
      </div>
      <div class="mb-4">
        <label class="block mb-2 font-medium">Content</label>
        <textarea v-model="newNote.content" class="input"></textarea>
      </div>
      <button class="btn btn-primary" type="submit">Create Note</button>
    </form>
    <table class="table-auto w-full border-collapse border border-gray-200">
      <thead>
        <tr>
          <th class="border border-gray-300 px-4 py-2">Title</th>
          <th class="border border-gray-300 px-4 py-2">Content</th>
          <th class="border border-gray-300 px-4 py-2">Created At</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="note in filteredNotes" :key="note.id" class="hover:bg-gray-100">
          <td class="border border-gray-300 px-4 py-2">{{ note.title }}</td>
          <td class="border border-gray-300 px-4 py-2">{{ note.content }}</td>
          <td class="border border-gray-300 px-4 py-2">{{ new Date(note.createdAt).toLocaleString() }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import api from '@/utils/api';

interface Note {
  id: number;
  title: string;
  content: string;
  createdAt: string;
}

export default defineComponent({
  data() {
    return {
      notes: [] as Note[],
      newNote: { title: '', content: '' },
      searchQuery: '',
    };
  },
  computed: {
    filteredNotes(): Note[] {
      return this.notes.filter(
        (note) =>
          note.title.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
          note.content.toLowerCase().includes(this.searchQuery.toLowerCase())
      );
    },
  },
  async created() {
    this.fetchNotes();
  },
  methods: {
    async fetchNotes() {
      try {
        const response = await api.get<Note[]>('/notes', {
          headers: { Authorization: `Bearer ${localStorage.getItem('token')}` },
        });
        this.notes = response.data;
      } catch (error) {
        console.error('Error fetching notes:', error);
        alert('Failed to fetch notes.');
      }
    },
    async handleCreateNote() {
      try {
        await api.post(
          '/notes',
          this.newNote,
          { headers: { Authorization: `Bearer ${localStorage.getItem('token')}` } }
        );
        this.newNote = { title: '', content: '' };
        this.fetchNotes();
      } catch (error) {
        console.error('Error creating note:', error);
        alert('Failed to create note.');
      }
    },
    logout() {
      localStorage.clear();
      this.$router.push('/login');
    },
  },
});
</script>


<style scoped>
.table-auto {
  border-spacing: 0;
}
.input {
  border: 1px solid #ccc;
  padding: 0.5rem;
  width: 100%;
}
.btn {
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-weight: bold;
}
.btn-primary {
  background-color: #007bff;
  color: white;
}
.btn-secondary {
  background-color: #6c757d;
  color: white;
}
</style>
