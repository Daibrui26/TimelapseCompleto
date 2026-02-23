<template>
  <div class="page">
    <AppHeader variant="app" />

    <main class="page__main">
      <section class="card card--profile">
        <img src="@/assets/img/Perfil.png" alt="Perfil" class="card__profile-img" />
        <div class="card__profile-info">
          <h2 class="card__title">Usuario</h2>
          <p class="card__subtitle">correousuario@gmail.com</p>
        </div>
      </section>

      <section class="card card--menu">
        <nav class="menu-nav">
          <RouterLink to="/perfil" class="menu-nav__item">
            <img src="@/assets/img/Perfil.png" class="menu-nav__icon" alt="Perfil" />
            Mi Perfil
          </RouterLink>
          <a href="#" class="menu-nav__item">
            <img src="@/assets/img/Stats.png" class="menu-nav__icon" alt="Stats" />
            Estadísticas de vida
          </a>
          <RouterLink to="/tus-capsulas" class="menu-nav__item">
            <img src="@/assets/img/Reloj.png" class="menu-nav__icon" alt="Cápsulas" />
            Tus Cápsulas
          </RouterLink>
          <a href="#" class="menu-nav__item">
            <img src="@/assets/img/Bandeja.png" class="menu-nav__icon" alt="Bandeja" />
            Bandeja de Entrada
          </a>
        </nav>
      </section>

      <section class="card card--menu">
        <nav class="menu-nav">
          <RouterLink to="/contacto" class="menu-nav__item">
            <img src="@/assets/img/Contacto.png" class="menu-nav__icon" alt="Contacto" />
            Contacto
          </RouterLink>
          <a href="#" class="menu-nav__item">
            <span class="menu-nav__star">★</span> Sobre Nosotros
          </a>
          <a href="#" class="menu-nav__item">
            <span class="menu-nav__star">★</span> Política &amp; Privacidad
          </a>
        </nav>
      </section>

      <section class="card card--menu">
        <nav class="menu-nav">
          <RouterLink to="/" class="menu-nav__item menu-nav__item--danger">
            <img src="@/assets/img/salir.png" class="menu-nav__icon" alt="Salir" />
            Cerrar Sesión
          </RouterLink>
          <a href="#" class="menu-nav__item menu-nav__item--danger">
            <img src="@/assets/img/Papelera.png" class="menu-nav__icon" alt="Eliminar" />
            Eliminar Cuenta
          </a>
        </nav>
      </section>
    </main>
  </div>
</template>

<script setup lang="ts">
import { RouterLink } from 'vue-router'
import AppHeader from '@/components/AppHeader.vue'
</script>