<template>
  <div class="top-user-menu d-flex align-items-center">
    <!-- Icono y nombre + flecha -->
    <div class="dropdown" ref="dropdown">
      <button
        ref="toggle"
        class="user-btn d-flex align-items-center"
        type="button"
        id="userDropdown"
        data-bs-toggle="dropdown"
        aria-expanded="false"
      >
        <slot name="icon">
          <i class="bi bi-person-circle fs-4"></i>
        </slot>
        <span class="user-name ms-2">{{ userName }}</span>
        <i class="bi bi-caret-down-fill ms-1"></i>
      </button>

      <ul class="dropdown-menu dropdown-menu-end shadow">
        <li>
          <button
            class="dropdown-item d-flex align-items-center"
            @click="onProfile"
          >
            <i class="bi bi-person me-2"></i> Perfil
          </button>
        </li>
        <li>
          <button
            class="dropdown-item d-flex align-items-center"
            @click="onPreferences"
          >
            <i class="bi bi-gear me-2"></i> Preferencias
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, onBeforeUnmount } from "vue";

export default {
  name: "TopUserMenu",
  props: {
    userName: { type: String, required: true },
  },
  emits: ["profile", "preferences"],
  setup(props, { emit }) {
    const toggle = ref(null);
    let bsDropdown = null;

    const onProfile = () => {
      emit("profile");
      bsDropdown?.hide();
    };
    const onPreferences = () => {
      emit("preferences");
      bsDropdown?.hide();
    };

    onMounted(() => {
      if (
        typeof window !== "undefined" &&
        window.bootstrap?.Dropdown &&
        toggle.value
      ) {
        bsDropdown = window.bootstrap.Dropdown.getOrCreateInstance(
          toggle.value
        );
      }
    });

    onBeforeUnmount(() => {
      if (bsDropdown) {
        bsDropdown.dispose();
        bsDropdown = null;
      }
    });

    return { toggle, onProfile, onPreferences };
  },
};
</script>
