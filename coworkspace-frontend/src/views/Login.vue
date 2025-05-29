<template>
  <div class="login-container">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-11 col-md-8 col-lg-5">
          <div class="card shadow border-0">
            <div class="card-body p-3">
              <h3 class="text-center fw-bold mb-3">Iniciar Sesión</h3>
              <form @submit.prevent="handleLogin" class="">
                <div class="mb-3 input-icon-wrapper">
                  <input
                    v-model="form.email"
                    type="email"
                    class="form-control"
                    id="email"
                    placeholder="tu@email.com"
                    required
                    :disabled="loading"
                  />
                  <i class="bi bi-person input-icon"></i>
                </div>
                <div class="mb-3 input-icon-wrapper">
                  <input
                    v-model="form.password"
                    type="password"
                    class="form-control"
                    id="password"
                    placeholder="••••••••"
                    required
                    :disabled="loading"
                  />
                  <i class="bi bi-lock input-icon"></i>
                </div>
                <div class="d-grid gap-2 mb-2">
                  <button type="submit" class="btn btn-primary" :disabled="loading">
                    <i class="bi bi-box-arrow-in-right me-2"></i>
                    {{ loading ? 'Iniciando...' : 'Iniciar Sesión' }}
                  </button>
                  <button type="button" class="btn btn-outline-secondary" @click="$emit('cancel')" :disabled="loading">
                    Volver
                  </button>
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
import { useAuthStore } from '../stores/auth';
import { useNotyf } from '../composables/useNotyf';

export default {
  name: 'LoginPage',
  data() {
    return {
      form: {
        email: '',
        password: '',
      },
      loading: false,
    };
  },
  setup() {
    const notyf = useNotyf();
    return { notyf };
  },
  methods: {
    async handleLogin() {
      this.loading = true;
      try {
        const authStore = useAuthStore();
        const message = await authStore.login({
          email: this.form.email,
          password: this.form.password,
        });
        this.notyf.success(message || 'Inicio de sesión exitoso');
        this.form.email = '';
        this.form.password = '';
        this.$emit('login-success');
      } catch (error) {
        // Intentamos obtener mensaje claro del error
        const errorMsg =
          error?.response?.data?.message ||
          error?.message ||
          'Error al iniciar sesión';
        this.notyf.error(errorMsg);
      } finally {
        this.loading = false;
      }
    },
  },
  emits: ['cancel', 'login-success'],
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
  height: 38px;
  padding-right: 2.5rem;
}

.btn {
  border-radius: 0.375rem;
  font-size: 0.9rem;
  height: 38px;
}

.alert-sm {
  font-size: 0.9rem;
  padding: 0.75rem;
  margin-bottom: 0;
  display: flex;
  align-items: center;
}

label {
  font-size: 0.9rem;
  font-weight: 500;
  display: block;
  margin-bottom: 0.25rem;
}

.input-icon-wrapper {
  position: relative;
}

.input-icon {
  position: absolute;
  top: 50%;
  right: 0.75rem;
  transform: translateY(-50%);
  color: #6c757d;
  pointer-events: none;
  font-size: 1.1rem;
}
</style>
