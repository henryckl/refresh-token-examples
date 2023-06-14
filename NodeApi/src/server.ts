import fastify from 'fastify'

const server = fastify()

server.get('/user', async (request, reply) => {
  return 'user name\n'
})

server.listen({ port: 3333 }, (err, address) => {
  if (err != null) {
    console.error(err)
    process.exit(1)
  }
  console.log(`Server listening at ${address}`)
})
