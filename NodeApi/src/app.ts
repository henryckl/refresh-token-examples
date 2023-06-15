import fastify from 'fastify'
import { env } from './env'
const app = fastify()

app.get('/user', async (request, reply) => {
  return 'user name\n'
})

app.listen({ port: env.PORT }, (err, address) => {
  if (err != null) {
    console.error(err)
    process.exit(1)
  }
  console.log(`Server listening at ${address}`)
})
