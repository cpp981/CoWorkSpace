<template>
  <div class="container reg-cont">
    <div class="row justify-content-center">
      <div class="col-11 col-md-8 col-lg-5">
        <div class="card shadow border-0">
          <div class="card-body p-3">
            <form @submit.prevent="onSubmit">
              <h2 class="mb-4">{{ props.title }}</h2>

              <div v-for="field in props.fields" :key="field.key" class="mb-3">
                <label :for="field.key" class="form-label">{{ field.label }}</label>

                <div class="input-group">
                  <span class="input-group-text">
                    <i :class="field.icon"></i>
                  </span>

                  <input
                    v-if="field.type !== 'select'"
                    :type="field.type"
                    class="form-control"
                    :id="field.key"
                    v-model="form[field.key]"
                    :placeholder="field.placeholder"
                    :required="field.required"
                    :disabled="loading"
                  />

                  <select
                    v-else
                    class="form-select"
                    :id="field.key"
                    v-model="form[field.key]"
                    :required="field.required"
                    :disabled="loading"
                  >
                    <option disabled value="">{{ field.placeholder }}</option>
                    <option v-for="option in field.options" :key="option.value" :value="option.value">
                      {{ option.label }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="d-flex justify-content-between">
                <button
                  type="button"
                  class="btn btn-secondary"
                  @click="emit('cancel')"
                  :disabled="loading"
                >
                  Cancelar
                </button>
                <button type="submit" class="btn btn-primary" :disabled="loading">
                  <span
                    v-if="loading"
                    class="spinner-border spinner-border-sm me-2"
                    role="status"
                    aria-hidden="true"
                  ></span>
                  {{ loading ? 'Registrando...' : props.submitButtonText }}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { reactive, ref, onMounted } from 'vue';
import { useNotyf } from '../composables/useNotyf';

const notyf = useNotyf();

const props = defineProps({
  title: String,
  fields: Array,
  submitButtonText: String,
  submitHandler: Function,
});
const emit = defineEmits(['cancel']);

const form = reactive({});
const loading = ref(false);

const initializeForm = () => {
  props.fields.forEach((field) => {
    form[field.key] = '';
  });
};

onMounted(() => {
  initializeForm();
});

const onSubmit = async () => {
  loading.value = true;
  try {
    const response = await props.submitHandler(form);
    const msg = response?.data?.message || 'Registro exitoso';
    notyf.success(msg);
    initializeForm();
  } catch (error) {
    const errorMsg =
      error?.response?.data?.message ||
      error?.message ||
      'Error al registrar';
    notyf.error(errorMsg);
  } finally {
    loading.value = false;
  }
};
</script>

<style scoped>
.reg-cont {
  font-family: 'Inter', sans-serif;
  font-weight: 400;
  font-size: 16px;
  line-height: 1.5;
}

/* Inputs y selects con altura fija para igualar login */
.form-control,
.form-select {
  height: 38px;
  font-size: 0.9rem;
  border-radius: 0.375rem;
  box-sizing: border-box;
}

/* Botones con altura y estilo igual que login */
.btn {
  border-radius: 0.375rem;
  font-size: 0.9rem;
  height: 38px;
  min-width: 120px;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
