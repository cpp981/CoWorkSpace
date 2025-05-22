<template>
  <div class="login-container">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-11 col-md-8 col-lg-5">
          <div class="card shadow border-0">
            <div class="card-body p-3">
              <h3 class="text-center fw-bold mb-3">
                Iniciar Sesión
              </h3>
              <form @submit.prevent="handleLogin" class="">
                <div class="mb-2">
                  <label for="email" class="form-label"><i class="bi bi-person me-1"></i>Email</label>
                  <input
                    v-model="form.email"
                    type="email"
                    class="form-control border border-dark"
                    id="email"
                    placeholder="tu@email.com"
                    required
                  />
                </div>
                <div class="mb-2">
                  <label for="password" class="form-label"><i class="bi bi-lock me-1"></i>Contraseña</label>
                  <input
                    v-model="form.password"
                    type="password"
                    class="form-control border border-dark"
                    id="password"
                    placeholder="••••••••"
                    required
                  />
                </div>
                <div class="d-grid gap-2 mb-2">
                  <button
                    type="submit"
                    class="btn btn-primary"
                    :disabled="loading"
                  >
                  <i class="bi bi-box-arrow-in-right me-2"></i>
                    {{ loading ? 'Iniciando...' : 'Iniciar Sesión' }}
                  </button>
                  <button
                    type="button"
                    class="btn btn-outline-secondary"
                    @click="$emit('cancel')"
                  >
                    Volver
                  </button>
                </div>
                <div v-if="error" class="alert alert-danger alert-sm" role="alert">
                  {{ error }}
                </div>
                <div v-if="success" class="alert alert-success alert-sm" role="alert">
                 ¡Inicio de sesión exitoso!
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'LoginPage',
  data() {
    return {
      form: {
        email: '',
        password: '',
      },
      loading: false,
      error: null,
      success: false,
    };
  },
  methods: {
    async handleLogin() {
      this.loading = true;
      this.error = null;
      this.success = false;
      try {
        const response = await axios.post('/api/auth/login', {
          email: this.form.email,
          password: this.form.password,
        });
        // Almacenar el token en localStorage (temporal, hasta usar Pinia)
        localStorage.setItem('token', response.data.token);
        this.success = true;
        this.form.email = '';
        this.form.password = '';
      } catch (error) {
        this.error = error.response?.data?.message || 'Error al iniciar sesión';
      } finally {
        this.loading = false;
      }
    },
  },
  emits: ['cancel'],
};
</script>

<style scoped>
.login-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #f8f9fa;
  min-width: 125vh;
}

.card {
  border-radius: 0.5rem;
  background: linear-gradient(to bottom, rgba(255, 255, 255, 0.95), rgba(255, 255, 255, 0.8));
}

.form-control {
  border-radius: 0.375rem;
  font-size: 0.9rem;
}

.btn {
  border-radius: 0.375rem;
  font-size: 0.9rem;
}

.alert-sm {
  font-size: 0.8rem;
  padding: 0.5rem;
  margin-bottom: 0;
}

label {
  font-size: 0.9rem;
}
</style>