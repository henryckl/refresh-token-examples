interface User {
  Name: string
}

function SayHello (user: User): void {
  console.log(`Hello, ${user.Name}`)
}

const user: User = {
  Name: 'Henryck'
}

SayHello(user)
