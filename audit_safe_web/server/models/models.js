const sequelize = require('../db')
const {DataTypes} = require('sequelize')

const User = sequelize.define('user',{
    id: {type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true},
    login: {type: DataTypes.STRING, unique: true},
    password: {type: DataTypes.STRING}
})

const Data = sequelize.define('data',{
    id: {type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true},
    domain: {type: DataTypes.STRING},
    user: {type: DataTypes.STRING},
    message: {type: DataTypes.STRING},
    date: {type: DataTypes.DATE},
    type: {type: DataTypes.STRING}
})

module.exports = {
    User,
    Data
}