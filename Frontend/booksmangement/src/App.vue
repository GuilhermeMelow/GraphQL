<template>
    <div>
        <div v-if="booksQueryLoading"> loading... </div>
        <div v-else-if="booksQueryError"> {{error}} </div>
        <ul v-else>
            <li v-for="book in books" :key="book.id">
                {{book.title}}
            </li>
        </ul>

        <label> AuthorId </label>
        <input v-model="authorId" type="text">

        <label> Titulo </label>
        <input v-model="model.title" type="text" />
        <button @click="publicar">Publicar</button>
    </div>
</template>

<script>
    import { mutations } from './mutations.gql'
    import { queries } from './queries.gql'
    import { useMutation, useQuery, useSubscription } from '@vue/apollo-composable'
    import { ref, watch } from 'vue-demi'

    export default {
        name: 'App',
        setup () {
            const books = ref([]);

            const authorId = ref('a7cc841a-a82a-43c5-bfc4-2293a83c1836')

            const model = {
                title: '',
                get authorId () { return authorId.value },
            }

            const { mutate: publishBook } = useMutation(mutations.addBook)

            const query = useQuery(queries.books)
            watch(query.result, data => {
                books.value = data.books.map(c => createBook(c))
            })

            const subscriptionQueryResult = useSubscription(queries.onBookAdded, { authorId });
            watch(subscriptionQueryResult.result, data => {
                books.value.push(createBook(data.bookPublished))
            })

            return {
                authorId,
                model,
                books,
                booksQueryLoading: query.loading,
                booksQueryError: query.error,
                publicar: () => publishBook({ model })
            }
        }
    }

    const createBook = (data) => {
        return {
            id: data.id,
            title: data.title
        }

    }
</script>

<style>
</style>
