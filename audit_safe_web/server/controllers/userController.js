const ApiError = require('../error/ApiError')
const bcrypt = require('bcrypt')
const jwt = require('jsonwebtoken')
const {User} = require('../models/models')
const sequelize = require("../db");
const {QueryTypes} = require("sequelize");

const generateJwt = (id, login) => {
    return jwt.sign(
        {id: id, login},
        process.env.SECRET_KEY,
        {expiresIn: '24h'}
    )
}

class  UserController{

    async registration(req, res, next) {
        const {login, password} = req.body
        if (!login || !password) {
            return next(ApiError.badRequest('Некоректный email или пароль'))
        }
        console.log(login)
        const candidate = await User.findOne({where: {login}});
        if (candidate) {
            return next(ApiError.badRequest('Пользователь с такой почтой уже есть'))
        }
        const hashPassword = await bcrypt.hash(password, 10)
        const user = await User.create({login, password: hashPassword})

        const token = generateJwt(user.id, user.login)
        return res.json({token})
    }

    async logon(req,res,next){
        const {login, password} = req.body
        const user = await User.findOne({where: {login}})
        if (!user){
            return next(ApiError.internal('Пользователь с таким именем не найден'))
        }
        let comparePassword = bcrypt.compareSync(password, user.password)
        if (!comparePassword){
            return next(ApiError.internal('Неверный пароль'))
        }
        const token = generateJwt(user.id, user.login)
        return res.json({token})
    }

    async check(req, res) {
        const token = generateJwt(req.user.id, req.user.login)
        return res.json({token})
    }
}

module.exports = new UserController()