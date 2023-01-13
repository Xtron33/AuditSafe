require('dotenv').config({path: __dirname+'/.env'})
const express = require('express')
const sequelize = require('./db')
const models = require('./models/models')
const cors = require('cors')
//const router = require('./routes/index')
//const errorHandler = require('./middleware/ErrorHadlingMiddleware')
const path = require('path')


const PORT = process.env.PORT

const app = express()
app.use(cors());
app.use(express.json())
app.use(express.static(path.resolve(__dirname, 'static')))
//app.use('/api', router)

//app.use(errorHandler)//Обработчик ошибок

const start = async() => {
    try{
        await sequelize.authenticate()
        await sequelize.sync()
        app.listen(PORT, () => console.log('Server started ' ))
    }
    catch(e){
        console.log(e)
    }
}
start()
