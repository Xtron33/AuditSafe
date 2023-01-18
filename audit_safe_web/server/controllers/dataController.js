const {Data} = require('../models/models')
const ApiError = require('../error/ApiError')
const sequelize = require("../db");
const Sequelize = require('sequelize');
const Op = Sequelize.Op;
const {QueryTypes} = require("sequelize");

class DataController{
    async create(req, res,next) {
        try {
            let {domain, user, message, date, type} = req.body
            const dataCheck = await Data.findOne({where: {domain, user, message, date, type}});
            if(dataCheck){
                return res.json('404')
            }
            else{
                const data = await Data.create({domain, user, message, date, type})
                return res.json(data)
            }


        }
        catch (e){
            next(ApiError.badRequest(e.message))
        }
    }
    async delete(req,res){
        const id = parseInt(req.params.id)

        await sequelize.query(
            'DELETE FROM data WHERE id = $1',
            {bind: [id], type:QueryTypes.DELETE},
            (error, results) =>
            {
                if(error){
                    throw error
                }
                res.status(200).send('Deleted')
            }
        )
        return res.json('Deleted')
    }
    async getAll(req, res) {

        let {domain} = req.query
        let data;
        if(!domain){
            data = await Data.findAll({})
        }
        if(domain){
            data = await Data.findAll({where: {domain}})
        }
        return res.json(data)
    }

}

module.exports = new DataController()