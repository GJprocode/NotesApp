<template>
  <div class="container mx-auto p-4">
    <h1 class="text-2xl font-bold mb-4">Register</h1>
    <form @submit.prevent="handleRegister">
      <div class="mb-4">
        <label class="block mb-2 font-medium">Username</label>
        <input v-model="username" type="text" class="input" placeholder="Enter your username" required />
      </div>
      <div class="mb-4">
        <label class="block mb-2 font-medium">Email</label>
        <input v-model="email" type="email" class="input" placeholder="Enter your email" required />
      </div>
      <div class="mb-4">
        <label class="block mb-2 font-medium">Password</label>
        <input v-model="password" type="password" class="input" placeholder="Enter your password" required />
      </div>
      <div class="mb-4">
        <label class="block mb-2 font-medium">Confirm Password</label>
        <input v-model="confirmPassword" type="password" class="input" placeholder="Confirm your password" required />
      </div>
      <button class="btn btn-primary" type="submit">Register</button>
    </form>
    <p class="mt-4 text-sm">
      Already a user?
      <router-link to="/login" class="text-blue-500 hover:underline">Login</router-link>
    </p>
  </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import api from '@/utils/api';
import axios from 'axios';

export default defineComponent({
  data() {
    return {
      username: '',
      email: '', // Add email property here
      password: '',
      confirmPassword: '',
    };
  },
  methods: {
    async handleRegister() {
      if (this.password !== this.confirmPassword) {
        alert('Passwords do not match!');
        return;
      }
      try {
        const response = await api.post('/auth/register', {
          username: this.username,
          email: this.email, // Send email to the backend
          password: this.password,
        });
        console.log('Registration successful:', response.data);
        alert('Registration successful! Please login.');
        this.$router.push('/login');
      } catch (error) {
        if (axios.isAxiosError(error)) {
          console.error('Axios error:', error.response?.data || error.message);
          if (error.response?.status === 409) {
            alert(error.response?.data?.Error || 'A user with this email or username already exists.');
          } else {
            alert('Registration failed. Please try again.');
          }
        } else {
          console.error('Unexpected error:', error);
          alert('An unexpected error occurred. Please try again.');
        }
      }
    },
  },
});
</script>
