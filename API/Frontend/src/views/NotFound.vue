<template>
  <div class="text-center my-5">
    <div class="display-1 text-muted mb-3">404</div>
    <h1 class="h3 mb-4">Page Not Found</h1>
    <p class="lead mb-4">The page you are looking for might have been removed, had its name changed, or is temporarily unavailable.</p>
    <router-link to="/" class="btn btn-primary">
      <i class="bi bi-house-door"></i> Go to Home
    </router-link>
  </div>
</template>

<script>
export default {
  name: 'NotFound'
};
</script> 