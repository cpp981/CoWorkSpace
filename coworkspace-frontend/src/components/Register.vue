<template>
  <div class="container reg-cont">
    <div class="row justify-content-center">
      <div class="col-11 col-md-8 col-lg-5">
        <div class="card shadow border-0">
          <div class="card-body p-3">
            <form @submit.prevent="onSubmit">
              <h2 class="mb-4">{{ props.title }}</h2>

              <div v-for="field in props.fields" :key="field.key" class="mb-3">
                <label :for="field.key" class="form-label">{{
                  field.label
                }}</label>

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
                    <option
                      v-for="option in field.options"
                      :key="option.value"
                      :value="option.value"
                    >
                      {{ option.label }}
                    </option>
                  </select>
                </div>
              </div>

              <div class="d-flex justify-content-between">
                <button
                  type="button"
                  class="btn btn-danger"
                  @click="emit('cancel')"
                  :disabled="loading"
                >
                  <i class="bi bi-x-circle me-2"></i>
                  Cancelar
                </button>
                <button
                  type="submit"
                  class="btn btn-primary"
                  :disabled="loading"
                >
                  <i class="bi bi-person-check-fill me-2"></i>
                  <span
                    v-if="loading"
                    class="spinner-border spinner-border-sm me-2"
                    role="status"
                    aria-hidden="true"
                  ></span>
                  {{ loading ? "Registrando..." : props.submitButtonText }}
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
import { reactive, ref, onMounted } from "vue";
import { useNotyf } from "../composables/useNotyf";

const notyf = useNotyf();

const props = defineProps({
  title: String,
  fields: Array,
  submitButtonText: String,
  submitHandler: Function,
});
const emit = defineEmits(["cancel"]);

const form = reactive({});
const loading = ref(false);

const initializeForm = () => {
  props.fields.forEach((field) => {
    form[field.key] = "";
  });
};

onMounted(() => {
  initializeForm();
});

const onSubmit = async () => {
  loading.value = true;
  try {
    const response = await props.submitHandler(form);
    const msg = response?.data?.message || "Registro exitoso";
    notyf.success(msg);
    initializeForm();
  } catch (error) {
    const errorMsg =
      error?.response?.data?.message || error?.message || "Error al registrar";
    notyf.error(errorMsg);
  } finally {
    loading.value = false;
  }
};
</script>
