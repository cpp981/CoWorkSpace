<template>
  <div class="register-container">
    <div class="container">
      <div class="row justify-content-center">
        <div class="col-11 col-md-8 col-lg-5">
          <div class="card shadow border-0">
            <div class="card-body p-3">
              <h3 class="text-center fw-bold mb-3">
                Registrarse
              </h3>
              <form @submit.prevent="handleRegister" class="">
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
                <div class="mb-2">
                  <label for="name" class="form-label"><i class="bi bi-person-circle me-1"></i>Nombre</label>
                  <input
                    v-model="form.name"
                    type="text"
                    class="form-control border border-dark"
                    id="name"
                    placeholder="Tu nombre"
                    required
                  />
                </div>
                <div class="mb-2">
                  <label for="role" class="form-label"><i class="bi bi-person-gear me-1"></i>Rol</label>
                  <select
                    v-model="form.roleId"
                    class="form-select border border-dark"
                    id="role"
                    required
                  >
                    <option value="" disabled>Selecciona un rol</option>
                    <option value="4">Cliente</option>
                    <option value="3">Proveedor</option>
                  </select>
                </div>
                <div class="d-grid gap-2 mb-2">
                  <button
                    type="submit"
                    class="btn btn-primary"
                    :disabled="loading"
                  >
                    <i class="bi bi-person-plus me-2"></i>
                    {{ loading ? 'Registrando...' : 'Registrarse' }}
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
                  <i class="bi bi-exclamation-circle me-2"></i>{{ error }}
                </div>
                <div v-if="success" class="alert alert-success alert-sm" role="alert">
                  <i class="bi bi-check-circle me-2"></i>{{ success }}
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
  name: 'RegisterPage',
  data() {
    return {
      form: {
        email: '',
        password: '',
        name: '',
        roleId: '',
      },
      loading: false,
      error: null,
      success: null,
    };
  },
  methods: {
    async handleRegister() {
      this.loading = true;
      this.error = null;
      this.success = null;

      try {
        const response = await axios.post('/api/auth/register', {
          email: this.form.email,
          password: this.form.password,
          name: this.form.name,
          roleId: parseInt(this.form.roleId), // Convertir a número
        });
        this.success = response.data.message;
        this.form.email = '';
        this.form.password = '';
        this.form.name = '';
        this.form.roleId = '';
      } catch (error) {
        this.error = error.response?.data?.message;
      } finally {
        this.loading = false;
      }
    },
  },
  emits: ['cancel'],
};
</script>

<style scoped>
.register-container {
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

.form-control,
.form-select {
  border-radius: 0.375rem;
  font-size: 0.9rem;
}

.btn {
  border-radius: 0.375rem;
  font-size: 0.9rem;
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
}
</style>