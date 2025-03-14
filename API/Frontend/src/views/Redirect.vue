<template>
  <div class="d-flex justify-content-center">
    <div class="spinner-border text-primary" role="status" v-if="loading">
      <span class="visually-hidden">Loading...</span>
    </div>
    
    <div class="alert alert-danger w-100" v-if="error">
      <h4 class="alert-heading">Error!</h4>
      <p>{{ error }}</p>
      <hr>
      <p class="mb-0">
        <router-link to="/" class="btn btn-primary">Back to Home</router-link>
      </p>
    </div>
  </div>
</template>

<script>
import urlService from '../services/urlService';

export default {
  name: 'Redirect',
  data() {
    return {
      loading: true,
      error: null
    };
  },
  async mounted() {
    const code = this.$route.params.code;
    
    try {
      const response = await urlService.getOriginalUrl(code);
      if (response && response.originalUrl) {
        window.location.href = response.originalUrl;
      } else {
        this.error = 'Invalid or expired short URL.';
        this.loading = false;
      }
    } catch (err) {
      this.error = err.response?.data?.message || 'This URL does not exist or has expired.';
      this.loading = false;
    }
  }
};
</script> 