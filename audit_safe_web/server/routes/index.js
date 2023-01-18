const express = require('express')
const router = express.Router()
const dataRouter = require('./dataRouter')
const userRouter = require('./userRouter')

router.use('/user', userRouter)
router.use('/data', dataRouter)

module.exports = router