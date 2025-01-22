<template>
  <div class="container mx-auto p-4">
    <h1 class="text-2xl font-bold mb-4">Welcome to the NotesAPP!</h1>
    <h2 class="text-2xl font-bold mb-4">Login</h2>
    <form @submit.prevent="handleLogin">
      <div class="mb-4">
        <label class="block mb-2 font-medium">Username</label>
        <input
          v-model="username"
          type="text"
          class="input"
          placeholder="Enter your username"
          required
        />
      </div>
      <div class="mb-4">
        <label class="block mb-2 font-medium">Password</label>
        <input
          v-model="password"
          type="password"
          class="input"
          placeholder="Enter your password"
          required
        />
      </div>
      <button class="btn btn-primary w-full" type="submit">Login</button>
    </form>
    <p class="mt-4 text-sm text-center">
      Not registered yet?
      <router-link to="/register" class="text-blue-500 hover:underline">Sign up here</router-link>.
    </p>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import axios from 'axios';
import api from '@/utils/api';

export default defineComponent({
  data() {
    return {
      username: '',
      password: '',
    };
  },
  methods: {
    async handleLogin() {
      try {
        const response = await api.post('/auth/login', {
          username: this.username,
          password: this.password,
        });

        // Save token and navigate to notes
        localStorage.setItem('token', response.data.token);
        alert('Login successful!');
        this.$router.push('/notes');
      } catch (error) {
        // Handle Axios-specific errors
        if (axios.isAxiosError(error)) {
          console.error('Axios error:', error.response?.data || error.message);
          alert(error.response?.data?.Error || 'Login failed. Please check your credentials.');
        } else if (error instanceof Error) {
          // General error handling
          console.error('General error:', error.message);
          alert('An unexpected error occurred. Please try again.');
        } else {
          console.error('Unexpected error:', error);
          alert('Unexpected error occurred.');
        }
      }
    },
  },
});
</script>

<style scoped>
.container {
  max-width: 400px;
  margin: 0 auto;
}
.input {
  border: 1px solid #ccc;
  padding: 0.5rem;
  width: 100%;
  border-radius: 4px;
  margin-top: 0.25rem;
}
.btn {
  padding: 0.5rem 1rem;
  border-radius: 4px;
  font-weight: bold;
  text-align: center;
}
.btn-primary {
  background-color: #007bff;
  color: white;
}
.btn-primary:hover {
  background-color: #0056b3;
}
.text-blue-500 {
  color: #007bff;
}
.text-blue-500:hover {
  text-decoration: underline;
}
</style>
