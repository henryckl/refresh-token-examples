interface User {
  Name: string
}

function SayHello (user: User) {
  console.log(`Hello, ${user.Name}`);
}

const user: User = {
  Name: "Henryck"
}

SayHello(user);
