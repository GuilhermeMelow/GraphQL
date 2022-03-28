import { DefaultApolloClient } from '@vue/apollo-composable'
import { createApp } from 'vue'
import { apolloClient } from './apolloClient'
import App from './App.vue'

createApp(App)
    .provide(DefaultApolloClient, apolloClient)
    .mount('#app')
