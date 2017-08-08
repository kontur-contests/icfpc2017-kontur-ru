const compression = require('compression');
const config = require('config');
const ect = require('ect');
const express = require('express');


const port = config.get('website.port');


process.on('unhandledRejection', err => console.error(err));


express()
    .use(compression())

    .use('/', express.static('static'))
    .use('/d3.js', express.static('node_modules/d3/build/d3.min.js'))
    .use('/moment.js', express.static('node_modules/moment/min/moment.min.js'))

    .get('/', (req, res) => res.render('index'))

    .set('view engine', 'ect')
    .engine('ect', ect({
        watch: true,
        root: `${__dirname}/../../views`
    }).render)

    .listen(port, () => console.info(`Website started at port ${port}`));