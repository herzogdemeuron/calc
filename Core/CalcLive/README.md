Back to [Calc](https://github.com/herzogdemeuron/calc#readme)

# CalcLive

CalcLive is the interactive visualization platform for Calc.

> CalcLive does not save any data it's recieving. Once the page is loaded in your browser everything happens locally.

## Technology

CalcLive is a basic **Vue.js** app that uses **Chart.js** for it's graphs. It uses a **Websocket Client** to listen on a certain port on your local machine.
Whenever it receives a message, it updates the charts.

You can send data to it from any application as long as you spin up a local websocket server
that broadcasts messages on the correct port. Obviously the data needs to have the correct format.
Take a look at the [Results](https://github.com/herzogdemeuron/calc/blob/master/Core/Core/Objects/DataStructures.cs) class in the Calc Core to see the required properties.

## Deployment

I think you can host this in any way you like - but:

Since CalcLive does not have any database access I recommend to hosting it on github pages.
If you don't care about the url, you can use any public deployment of CalcLive to visualize your data.

### Update GitHub Pages

1. Merge all required changes into the branch you want to deploy from (e.g. gh-pages)
2. Run `npm run build`
3. Copy the build output into calc/docs (if you choose to deploy from the docs folder)
4. Commit and push

# For Developers

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).
