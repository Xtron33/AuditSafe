const express = require("express");
const router = express.Router()
const userController = require('../controllers/userController')
const authMiddleware = require('../middleware/AuthMiddleware')

router.post('/registration', userController.registration)
router.post('/login', userController.logon)
router.get('/auth', authMiddleware, userController.check)

module.exports = router