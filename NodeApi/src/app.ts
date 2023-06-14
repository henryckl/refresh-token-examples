import fastify from 'fastify'

const app = fastify()

app.get('/user', async (request, reply) => {
  return 'user name\n'
})

app.listen({ port: 3333 }, (err, address) => {
  if (err != null) {
    console.error(err)
    process.exit(1)
  }
  console.log(`Server listening at ${address}`)
})
