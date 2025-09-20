/*
 Navicat Premium Data Transfer

 Source Server         : lu
 Source Server Type    : MySQL
 Source Server Version : 80036
 Source Host           : 192.168.24.35:3306
 Source Schema         : hospitalprinting

 Target Server Type    : MySQL
 Target Server Version : 80036
 File Encoding         : 65001

 Date: 20/09/2025 17:24:14
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for hol_examination
-- ----------------------------
DROP TABLE IF EXISTS `hol_examination`;
CREATE TABLE `hol_examination`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `exam_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '检查号（唯一标识）',
  `patient_id` bigint NOT NULL COMMENT '患者ID',
  `org_id` bigint NOT NULL COMMENT '所属机构ID',
  `exam_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '检查类型（CT/MRI/超声等）',
  `exam_date` datetime NOT NULL COMMENT '检查日期',
  `report_path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '报告文件路径',
  `image_path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '电子胶片路径',
  `status` tinyint NULL DEFAULT 1 COMMENT '状态：1=有效，0=过期/删除',
  `create_time` datetime NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` datetime NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `is_printed` tinyint NULL DEFAULT 0 COMMENT '是否已打印：0=未打印，1=已打印',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `uk_exam_no`(`exam_no` ASC) USING BTREE,
  INDEX `idx_patient_id`(`patient_id` ASC) USING BTREE,
  INDEX `idx_org_id`(`org_id` ASC) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '检查数据表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of hol_examination
-- ----------------------------
INSERT INTO `hol_examination` VALUES (1, '1', 2, 1, 'CT', '2025-09-20 00:32:27', '啊', '1', 1, '2025-09-20 16:32:39', '2025-09-20 16:39:49', 1);

-- ----------------------------
-- Table structure for hol_patient
-- ----------------------------
DROP TABLE IF EXISTS `hol_patient`;
CREATE TABLE `hol_patient`  (
  `id` bigint NOT NULL AUTO_INCREMENT,
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '患者姓名',
  `gender` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '性别',
  `age` int NULL DEFAULT NULL COMMENT '年龄',
  `id_card` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '身份证号',
  `contact` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '联系方式',
  `medical_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '就诊号',
  `createtime` datetime NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updatetime` datetime NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '修改时间',
  `status` tinyint NULL DEFAULT 1 COMMENT '是否有效（0，1）',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `id_card`(`id_card` ASC) USING BTREE,
  UNIQUE INDEX `medical_no`(`medical_no` ASC) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of hol_patient
-- ----------------------------
INSERT INTO `hol_patient` VALUES (1, '张三', '女', 25, '111111111111111111', '11111111112', 'bh1111', '2025-09-20 14:53:19', '2025-09-20 15:08:52', 1);
INSERT INTO `hol_patient` VALUES (2, '李四', '男', 26, '111', '44', '44', '2025-09-20 16:18:27', '2025-09-20 16:18:27', 1);

-- ----------------------------
-- Table structure for hol_print_record
-- ----------------------------
DROP TABLE IF EXISTS `hol_print_record`;
CREATE TABLE `hol_print_record`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `exam_id` bigint NOT NULL COMMENT '检查数据ID',
  `patient_id` bigint NOT NULL COMMENT '患者ID',
  `print_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '打印时间',
  `printed_by` bigint NOT NULL COMMENT '打印人ID（患者ID本身）',
  `status` tinyint NOT NULL DEFAULT 1 COMMENT '状态：1=有效，0=过期/删除',
  `create_time` datetime NOT NULL COMMENT '创建时间',
  `update_time` datetime NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `uk_exam_id`(`exam_id` ASC) USING BTREE,
  INDEX `fk_print_patient`(`patient_id` ASC) USING BTREE,
  CONSTRAINT `fk_print_exam` FOREIGN KEY (`exam_id`) REFERENCES `hol_examination` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_print_patient` FOREIGN KEY (`patient_id`) REFERENCES `hol_patient` (`id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '患者自助打印记录表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of hol_print_record
-- ----------------------------
INSERT INTO `hol_print_record` VALUES (3, 1, 2, '2025-09-20 16:39:49', 2, 1, '2025-09-20 16:39:49', '2025-09-20 16:39:49');

-- ----------------------------
-- Table structure for sys_examination
-- ----------------------------
DROP TABLE IF EXISTS `sys_examination`;
CREATE TABLE `sys_examination`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '主键ID',
  `exam_no` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '检查号（唯一标识）',
  `patient_id` bigint NOT NULL COMMENT '患者ID',
  `org_id` bigint NULL DEFAULT NULL COMMENT '所属机构ID',
  `exam_type` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '检查类型（CT/MRI/超声等）',
  `exam_date` datetime NOT NULL COMMENT '检查日期',
  `report_path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '报告文件路径',
  `image_path` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '电子胶片路径',
  `status` tinyint NULL DEFAULT 1 COMMENT '状态：1=有效，0=过期/删除',
  `create_time` datetime NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `update_time` datetime NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  PRIMARY KEY (`id`) USING BTREE,
  UNIQUE INDEX `uk_exam_no`(`exam_no` ASC) USING BTREE,
  INDEX `idx_patient_id`(`patient_id` ASC) USING BTREE,
  INDEX `idx_org_id`(`org_id` ASC) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '检查数据表' ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of sys_examination
-- ----------------------------

-- ----------------------------
-- Table structure for sys_operationlog
-- ----------------------------
DROP TABLE IF EXISTS `sys_operationlog`;
CREATE TABLE `sys_operationlog`  (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `UserId` int NOT NULL COMMENT '操作人',
  `ActionType` int NOT NULL COMMENT '操作类型',
  `ModuleName` varchar(500) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '模块名称',
  `Description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '操作描述',
  `ActionTime` datetime NOT NULL COMMENT '操作时间',
  `ActionContent` varchar(5000) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '操作内容',
  `OrgId` int NOT NULL COMMENT '服务器Id',
  `AddUserId` int NOT NULL COMMENT '创建人',
  `AddTime` datetime NOT NULL COMMENT '创建时间',
  `UpUserId` int NOT NULL COMMENT '更新人',
  `UpTime` datetime NOT NULL COMMENT '更新时间',
  PRIMARY KEY (`Id`) USING BTREE,
  INDEX `index`(`UserId` ASC, `ActionType` ASC, `ModuleName` ASC, `OrgId` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 7736 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '系统操作日志' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_operationlog
-- ----------------------------
INSERT INTO `sys_operationlog` VALUES (7300, 0, 4, '系统管理>定时任务', '定时任务详情', '2025-09-19 13:47:44', '{\"taskId\":1}', 0, 0, '2025-09-19 13:47:44', 0, '2025-09-19 13:47:44');
INSERT INTO `sys_operationlog` VALUES (7301, 1, 10, '系统登陆', '人员登陆', '2025-09-19 14:09:26', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 14:09:26', 1, '2025-09-19 14:09:26');
INSERT INTO `sys_operationlog` VALUES (7302, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:10:49', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:10:49', 1, '2025-09-19 14:10:49');
INSERT INTO `sys_operationlog` VALUES (7303, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:10:49', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:10:49', 1, '2025-09-19 14:10:49');
INSERT INTO `sys_operationlog` VALUES (7304, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:10:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:10:50', 1, '2025-09-19 14:10:50');
INSERT INTO `sys_operationlog` VALUES (7305, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:10:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:10:50', 1, '2025-09-19 14:10:50');
INSERT INTO `sys_operationlog` VALUES (7306, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:10:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:10:50', 1, '2025-09-19 14:10:50');
INSERT INTO `sys_operationlog` VALUES (7307, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:11:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:11:39', 1, '2025-09-19 14:11:39');
INSERT INTO `sys_operationlog` VALUES (7308, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:11:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:11:39', 1, '2025-09-19 14:11:39');
INSERT INTO `sys_operationlog` VALUES (7309, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:11:40', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:11:40', 1, '2025-09-19 14:11:40');
INSERT INTO `sys_operationlog` VALUES (7310, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:11:40', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:11:40', 1, '2025-09-19 14:11:40');
INSERT INTO `sys_operationlog` VALUES (7311, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:11:40', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:11:40', 1, '2025-09-19 14:11:40');
INSERT INTO `sys_operationlog` VALUES (7312, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:45', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:45', 1, '2025-09-19 14:11:45');
INSERT INTO `sys_operationlog` VALUES (7313, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:45', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:45', 1, '2025-09-19 14:11:45');
INSERT INTO `sys_operationlog` VALUES (7314, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:45', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:45', 1, '2025-09-19 14:11:45');
INSERT INTO `sys_operationlog` VALUES (7315, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:45', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:45', 1, '2025-09-19 14:11:45');
INSERT INTO `sys_operationlog` VALUES (7316, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:46', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:46', 1, '2025-09-19 14:11:46');
INSERT INTO `sys_operationlog` VALUES (7317, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:46', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:46', 1, '2025-09-19 14:11:46');
INSERT INTO `sys_operationlog` VALUES (7318, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:46', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:46', 1, '2025-09-19 14:11:46');
INSERT INTO `sys_operationlog` VALUES (7319, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:47', 1, '2025-09-19 14:11:47');
INSERT INTO `sys_operationlog` VALUES (7320, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:47', 1, '2025-09-19 14:11:47');
INSERT INTO `sys_operationlog` VALUES (7321, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:48', 1, '2025-09-19 14:11:48');
INSERT INTO `sys_operationlog` VALUES (7322, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:48', 1, '2025-09-19 14:11:48');
INSERT INTO `sys_operationlog` VALUES (7323, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:48', 1, '2025-09-19 14:11:48');
INSERT INTO `sys_operationlog` VALUES (7324, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:48', 1, '2025-09-19 14:11:48');
INSERT INTO `sys_operationlog` VALUES (7325, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:11:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:11:48', 1, '2025-09-19 14:11:48');
INSERT INTO `sys_operationlog` VALUES (7326, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:49', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:49', 1, '2025-09-19 14:11:49');
INSERT INTO `sys_operationlog` VALUES (7327, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:49', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:49', 1, '2025-09-19 14:11:49');
INSERT INTO `sys_operationlog` VALUES (7328, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:49', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:49', 1, '2025-09-19 14:11:49');
INSERT INTO `sys_operationlog` VALUES (7329, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:49', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:49', 1, '2025-09-19 14:11:49');
INSERT INTO `sys_operationlog` VALUES (7330, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:49', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:49', 1, '2025-09-19 14:11:49');
INSERT INTO `sys_operationlog` VALUES (7331, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:58', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:58', 1, '2025-09-19 14:11:58');
INSERT INTO `sys_operationlog` VALUES (7332, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:11:58', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:11:58', 1, '2025-09-19 14:11:58');
INSERT INTO `sys_operationlog` VALUES (7333, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:12:04', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":2,\"Size\":10}', 1, 1, '2025-09-19 14:12:04', 1, '2025-09-19 14:12:04');
INSERT INTO `sys_operationlog` VALUES (7334, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:12:05', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":3,\"Size\":10}', 1, 1, '2025-09-19 14:12:05', 1, '2025-09-19 14:12:05');
INSERT INTO `sys_operationlog` VALUES (7335, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:12:06', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":4,\"Size\":10}', 1, 1, '2025-09-19 14:12:06', 1, '2025-09-19 14:12:06');
INSERT INTO `sys_operationlog` VALUES (7336, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:12:07', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":3,\"Size\":10}', 1, 1, '2025-09-19 14:12:07', 1, '2025-09-19 14:12:07');
INSERT INTO `sys_operationlog` VALUES (7337, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:12:08', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":2,\"Size\":10}', 1, 1, '2025-09-19 14:12:08', 1, '2025-09-19 14:12:08');
INSERT INTO `sys_operationlog` VALUES (7338, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:12:09', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:12:09', 1, '2025-09-19 14:12:09');
INSERT INTO `sys_operationlog` VALUES (7339, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:12:13', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:12:13', 1, '2025-09-19 14:12:13');
INSERT INTO `sys_operationlog` VALUES (7340, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:12:13', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:12:13', 1, '2025-09-19 14:12:13');
INSERT INTO `sys_operationlog` VALUES (7341, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:12:13', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:12:13', 1, '2025-09-19 14:12:13');
INSERT INTO `sys_operationlog` VALUES (7342, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:13:22', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:13:22', 1, '2025-09-19 14:13:22');
INSERT INTO `sys_operationlog` VALUES (7343, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:13:22', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:13:22', 1, '2025-09-19 14:13:22');
INSERT INTO `sys_operationlog` VALUES (7344, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:13:22', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:13:22', 1, '2025-09-19 14:13:22');
INSERT INTO `sys_operationlog` VALUES (7345, 1, 11, '账号登出', '人员退出系统', '2025-09-19 14:14:58', '{}', 1, 1, '2025-09-19 14:14:58', 1, '2025-09-19 14:14:58');
INSERT INTO `sys_operationlog` VALUES (7346, 1, 10, '系统登陆', '人员登陆', '2025-09-19 14:37:27', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 14:37:27', 1, '2025-09-19 14:37:27');
INSERT INTO `sys_operationlog` VALUES (7347, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:41:25', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:41:25', 1, '2025-09-19 14:41:25');
INSERT INTO `sys_operationlog` VALUES (7348, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:41:25', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:41:25', 1, '2025-09-19 14:41:25');
INSERT INTO `sys_operationlog` VALUES (7349, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:41:25', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:41:25', 1, '2025-09-19 14:41:25');
INSERT INTO `sys_operationlog` VALUES (7350, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:41:26', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:41:26', 1, '2025-09-19 14:41:26');
INSERT INTO `sys_operationlog` VALUES (7351, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:41:26', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:41:26', 1, '2025-09-19 14:41:26');
INSERT INTO `sys_operationlog` VALUES (7352, 1, 11, '账号登出', '人员退出系统', '2025-09-19 14:42:56', '{}', 1, 1, '2025-09-19 14:42:56', 1, '2025-09-19 14:42:56');
INSERT INTO `sys_operationlog` VALUES (7353, 1, 10, '系统登陆', '人员登陆', '2025-09-19 14:43:04', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 14:43:04', 1, '2025-09-19 14:43:04');
INSERT INTO `sys_operationlog` VALUES (7354, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:49:18', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:49:18', 1, '2025-09-19 14:49:18');
INSERT INTO `sys_operationlog` VALUES (7355, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:49:18', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:49:18', 1, '2025-09-19 14:49:18');
INSERT INTO `sys_operationlog` VALUES (7356, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:50:07', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:07', 1, '2025-09-19 14:50:07');
INSERT INTO `sys_operationlog` VALUES (7357, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:50:07', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:07', 1, '2025-09-19 14:50:07');
INSERT INTO `sys_operationlog` VALUES (7358, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:50:07', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:07', 1, '2025-09-19 14:50:07');
INSERT INTO `sys_operationlog` VALUES (7359, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:14', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:14', 1, '2025-09-19 14:50:14');
INSERT INTO `sys_operationlog` VALUES (7360, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:14', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:14', 1, '2025-09-19 14:50:14');
INSERT INTO `sys_operationlog` VALUES (7361, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:14', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:14', 1, '2025-09-19 14:50:14');
INSERT INTO `sys_operationlog` VALUES (7362, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:50:15', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:15', 1, '2025-09-19 14:50:15');
INSERT INTO `sys_operationlog` VALUES (7363, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:50:15', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:15', 1, '2025-09-19 14:50:15');
INSERT INTO `sys_operationlog` VALUES (7364, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:50:15', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:15', 1, '2025-09-19 14:50:15');
INSERT INTO `sys_operationlog` VALUES (7365, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:19', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:19', 1, '2025-09-19 14:50:19');
INSERT INTO `sys_operationlog` VALUES (7366, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:19', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:19', 1, '2025-09-19 14:50:19');
INSERT INTO `sys_operationlog` VALUES (7367, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:19', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:19', 1, '2025-09-19 14:50:19');
INSERT INTO `sys_operationlog` VALUES (7368, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:19', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:19', 1, '2025-09-19 14:50:19');
INSERT INTO `sys_operationlog` VALUES (7369, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:19', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:19', 1, '2025-09-19 14:50:19');
INSERT INTO `sys_operationlog` VALUES (7370, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:50:21', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:50:21', 1, '2025-09-19 14:50:21');
INSERT INTO `sys_operationlog` VALUES (7371, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:50:21', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:50:21', 1, '2025-09-19 14:50:21');
INSERT INTO `sys_operationlog` VALUES (7372, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:50:21', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:50:21', 1, '2025-09-19 14:50:21');
INSERT INTO `sys_operationlog` VALUES (7373, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:50:21', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:50:21', 1, '2025-09-19 14:50:21');
INSERT INTO `sys_operationlog` VALUES (7374, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:50:21', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:50:21', 1, '2025-09-19 14:50:21');
INSERT INTO `sys_operationlog` VALUES (7375, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:50:21', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:50:21', 1, '2025-09-19 14:50:21');
INSERT INTO `sys_operationlog` VALUES (7376, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:22', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:22', 1, '2025-09-19 14:50:22');
INSERT INTO `sys_operationlog` VALUES (7377, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:22', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:22', 1, '2025-09-19 14:50:22');
INSERT INTO `sys_operationlog` VALUES (7378, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:22', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:22', 1, '2025-09-19 14:50:22');
INSERT INTO `sys_operationlog` VALUES (7379, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:22', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:22', 1, '2025-09-19 14:50:22');
INSERT INTO `sys_operationlog` VALUES (7380, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:22', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:22', 1, '2025-09-19 14:50:22');
INSERT INTO `sys_operationlog` VALUES (7381, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:50:22', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:50:22', 1, '2025-09-19 14:50:22');
INSERT INTO `sys_operationlog` VALUES (7382, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:38', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:38', 1, '2025-09-19 14:50:38');
INSERT INTO `sys_operationlog` VALUES (7383, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:38', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:38', 1, '2025-09-19 14:50:38');
INSERT INTO `sys_operationlog` VALUES (7384, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:38', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:38', 1, '2025-09-19 14:50:38');
INSERT INTO `sys_operationlog` VALUES (7385, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:38', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:38', 1, '2025-09-19 14:50:38');
INSERT INTO `sys_operationlog` VALUES (7386, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:38', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:38', 1, '2025-09-19 14:50:38');
INSERT INTO `sys_operationlog` VALUES (7387, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:50:38', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:50:38', 1, '2025-09-19 14:50:38');
INSERT INTO `sys_operationlog` VALUES (7388, 1, 11, '账号登出', '人员退出系统', '2025-09-19 14:51:04', '{}', 1, 1, '2025-09-19 14:51:04', 1, '2025-09-19 14:51:04');
INSERT INTO `sys_operationlog` VALUES (7389, 1, 10, '系统登陆', '人员登陆', '2025-09-19 14:51:10', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 14:51:10', 1, '2025-09-19 14:51:10');
INSERT INTO `sys_operationlog` VALUES (7390, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:51:26', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:51:26', 1, '2025-09-19 14:51:26');
INSERT INTO `sys_operationlog` VALUES (7391, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:51:26', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:51:26', 1, '2025-09-19 14:51:26');
INSERT INTO `sys_operationlog` VALUES (7392, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:51:27', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:51:27', 1, '2025-09-19 14:51:27');
INSERT INTO `sys_operationlog` VALUES (7393, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:51:27', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:51:27', 1, '2025-09-19 14:51:27');
INSERT INTO `sys_operationlog` VALUES (7394, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:51:27', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:51:27', 1, '2025-09-19 14:51:27');
INSERT INTO `sys_operationlog` VALUES (7395, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:51:28', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:51:28', 1, '2025-09-19 14:51:28');
INSERT INTO `sys_operationlog` VALUES (7396, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:51:28', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:51:28', 1, '2025-09-19 14:51:28');
INSERT INTO `sys_operationlog` VALUES (7397, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:51:28', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:51:28', 1, '2025-09-19 14:51:28');
INSERT INTO `sys_operationlog` VALUES (7398, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:51:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:51:50', 1, '2025-09-19 14:51:50');
INSERT INTO `sys_operationlog` VALUES (7399, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:51:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:51:50', 1, '2025-09-19 14:51:50');
INSERT INTO `sys_operationlog` VALUES (7400, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:51:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:51:50', 1, '2025-09-19 14:51:50');
INSERT INTO `sys_operationlog` VALUES (7401, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:51:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:51:50', 1, '2025-09-19 14:51:50');
INSERT INTO `sys_operationlog` VALUES (7402, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:51:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:51:50', 1, '2025-09-19 14:51:50');
INSERT INTO `sys_operationlog` VALUES (7403, 1, 1, '系统设置>角色管理', '修改角色权限', '2025-09-19 14:51:56', '{\"roleId\":1,\"permissionIds\":[12,86,87,93,95]}', 1, 1, '2025-09-19 14:51:56', 1, '2025-09-19 14:51:56');
INSERT INTO `sys_operationlog` VALUES (7404, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:51:59', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:51:59', 1, '2025-09-19 14:51:59');
INSERT INTO `sys_operationlog` VALUES (7405, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:02', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:02', 1, '2025-09-19 14:52:02');
INSERT INTO `sys_operationlog` VALUES (7406, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:02', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:02', 1, '2025-09-19 14:52:02');
INSERT INTO `sys_operationlog` VALUES (7407, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:03', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:03', 1, '2025-09-19 14:52:03');
INSERT INTO `sys_operationlog` VALUES (7408, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:03', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:03', 1, '2025-09-19 14:52:03');
INSERT INTO `sys_operationlog` VALUES (7409, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:03', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:03', 1, '2025-09-19 14:52:03');
INSERT INTO `sys_operationlog` VALUES (7410, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:03', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:03', 1, '2025-09-19 14:52:03');
INSERT INTO `sys_operationlog` VALUES (7411, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:03', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:03', 1, '2025-09-19 14:52:03');
INSERT INTO `sys_operationlog` VALUES (7412, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:04', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:04', 1, '2025-09-19 14:52:04');
INSERT INTO `sys_operationlog` VALUES (7413, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:04', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:04', 1, '2025-09-19 14:52:04');
INSERT INTO `sys_operationlog` VALUES (7414, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:04', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:04', 1, '2025-09-19 14:52:04');
INSERT INTO `sys_operationlog` VALUES (7415, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:04', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:04', 1, '2025-09-19 14:52:04');
INSERT INTO `sys_operationlog` VALUES (7416, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:05', 1, '2025-09-19 14:52:05');
INSERT INTO `sys_operationlog` VALUES (7417, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:05', 1, '2025-09-19 14:52:05');
INSERT INTO `sys_operationlog` VALUES (7418, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:05', 1, '2025-09-19 14:52:05');
INSERT INTO `sys_operationlog` VALUES (7419, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:05', 1, '2025-09-19 14:52:05');
INSERT INTO `sys_operationlog` VALUES (7420, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:11', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:11', 1, '2025-09-19 14:52:11');
INSERT INTO `sys_operationlog` VALUES (7421, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:11', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:11', 1, '2025-09-19 14:52:11');
INSERT INTO `sys_operationlog` VALUES (7422, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:11', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:11', 1, '2025-09-19 14:52:11');
INSERT INTO `sys_operationlog` VALUES (7423, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:11', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:11', 1, '2025-09-19 14:52:11');
INSERT INTO `sys_operationlog` VALUES (7424, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:14', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:14', 1, '2025-09-19 14:52:14');
INSERT INTO `sys_operationlog` VALUES (7425, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:17', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:17', 1, '2025-09-19 14:52:17');
INSERT INTO `sys_operationlog` VALUES (7426, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:17', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:17', 1, '2025-09-19 14:52:17');
INSERT INTO `sys_operationlog` VALUES (7427, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:18', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:18', 1, '2025-09-19 14:52:18');
INSERT INTO `sys_operationlog` VALUES (7428, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:18', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:18', 1, '2025-09-19 14:52:18');
INSERT INTO `sys_operationlog` VALUES (7429, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:18', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:18', 1, '2025-09-19 14:52:18');
INSERT INTO `sys_operationlog` VALUES (7430, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:18', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:18', 1, '2025-09-19 14:52:18');
INSERT INTO `sys_operationlog` VALUES (7431, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:18', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:18', 1, '2025-09-19 14:52:18');
INSERT INTO `sys_operationlog` VALUES (7432, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:19', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:19', 1, '2025-09-19 14:52:19');
INSERT INTO `sys_operationlog` VALUES (7433, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:19', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:19', 1, '2025-09-19 14:52:19');
INSERT INTO `sys_operationlog` VALUES (7434, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:19', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:19', 1, '2025-09-19 14:52:19');
INSERT INTO `sys_operationlog` VALUES (7435, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 14:52:19', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 14:52:19', 1, '2025-09-19 14:52:19');
INSERT INTO `sys_operationlog` VALUES (7436, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:20', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:20', 1, '2025-09-19 14:52:20');
INSERT INTO `sys_operationlog` VALUES (7437, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:20', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:20', 1, '2025-09-19 14:52:20');
INSERT INTO `sys_operationlog` VALUES (7438, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:20', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:20', 1, '2025-09-19 14:52:20');
INSERT INTO `sys_operationlog` VALUES (7439, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 14:52:20', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 14:52:20', 1, '2025-09-19 14:52:20');
INSERT INTO `sys_operationlog` VALUES (7440, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:20', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:20', 1, '2025-09-19 14:52:20');
INSERT INTO `sys_operationlog` VALUES (7441, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:21', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:21', 1, '2025-09-19 14:52:21');
INSERT INTO `sys_operationlog` VALUES (7442, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:21', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:21', 1, '2025-09-19 14:52:21');
INSERT INTO `sys_operationlog` VALUES (7443, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:21', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:21', 1, '2025-09-19 14:52:21');
INSERT INTO `sys_operationlog` VALUES (7444, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:21', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:21', 1, '2025-09-19 14:52:21');
INSERT INTO `sys_operationlog` VALUES (7445, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:21', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:21', 1, '2025-09-19 14:52:21');
INSERT INTO `sys_operationlog` VALUES (7446, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:21', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:21', 1, '2025-09-19 14:52:21');
INSERT INTO `sys_operationlog` VALUES (7447, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 14:52:21', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:21', 1, '2025-09-19 14:52:21');
INSERT INTO `sys_operationlog` VALUES (7448, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:22', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:22', 1, '2025-09-19 14:52:22');
INSERT INTO `sys_operationlog` VALUES (7449, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:22', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:22', 1, '2025-09-19 14:52:22');
INSERT INTO `sys_operationlog` VALUES (7450, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:22', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:22', 1, '2025-09-19 14:52:22');
INSERT INTO `sys_operationlog` VALUES (7451, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:22', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:22', 1, '2025-09-19 14:52:22');
INSERT INTO `sys_operationlog` VALUES (7452, 1, 1, '系统设置>角色管理', '修改角色权限', '2025-09-19 14:52:44', '{\"roleId\":1,\"permissionIds\":[12,86,87,88,93,95]}', 1, 1, '2025-09-19 14:52:44', 1, '2025-09-19 14:52:44');
INSERT INTO `sys_operationlog` VALUES (7453, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 14:52:46', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 14:52:46', 1, '2025-09-19 14:52:46');
INSERT INTO `sys_operationlog` VALUES (7454, 1, 10, '系统登陆', '人员登陆', '2025-09-19 14:59:56', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 14:59:56', 1, '2025-09-19 14:59:56');
INSERT INTO `sys_operationlog` VALUES (7455, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:00:03', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:00:03', 1, '2025-09-19 15:00:03');
INSERT INTO `sys_operationlog` VALUES (7456, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:00:03', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:00:03', 1, '2025-09-19 15:00:03');
INSERT INTO `sys_operationlog` VALUES (7457, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:00:04', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:00:04', 1, '2025-09-19 15:00:04');
INSERT INTO `sys_operationlog` VALUES (7458, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:00:04', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:00:04', 1, '2025-09-19 15:00:04');
INSERT INTO `sys_operationlog` VALUES (7459, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:00:04', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:00:04', 1, '2025-09-19 15:00:04');
INSERT INTO `sys_operationlog` VALUES (7460, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:00:06', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:00:06', 1, '2025-09-19 15:00:06');
INSERT INTO `sys_operationlog` VALUES (7461, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:00:06', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:00:06', 1, '2025-09-19 15:00:06');
INSERT INTO `sys_operationlog` VALUES (7462, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:00:06', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:00:06', 1, '2025-09-19 15:00:06');
INSERT INTO `sys_operationlog` VALUES (7463, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:00:06', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:00:06', 1, '2025-09-19 15:00:06');
INSERT INTO `sys_operationlog` VALUES (7464, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:00:06', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:00:06', 1, '2025-09-19 15:00:06');
INSERT INTO `sys_operationlog` VALUES (7465, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:00:07', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:00:07', 1, '2025-09-19 15:00:07');
INSERT INTO `sys_operationlog` VALUES (7466, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:00:07', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:00:07', 1, '2025-09-19 15:00:07');
INSERT INTO `sys_operationlog` VALUES (7467, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:00:07', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:00:07', 1, '2025-09-19 15:00:07');
INSERT INTO `sys_operationlog` VALUES (7468, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:00:07', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:00:07', 1, '2025-09-19 15:00:07');
INSERT INTO `sys_operationlog` VALUES (7469, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:00:07', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:00:07', 1, '2025-09-19 15:00:07');
INSERT INTO `sys_operationlog` VALUES (7470, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:00:07', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:00:07', 1, '2025-09-19 15:00:07');
INSERT INTO `sys_operationlog` VALUES (7471, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:00:10', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:00:10', 1, '2025-09-19 15:00:10');
INSERT INTO `sys_operationlog` VALUES (7472, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:04', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:04', 1, '2025-09-19 15:02:04');
INSERT INTO `sys_operationlog` VALUES (7473, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:04', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:04', 1, '2025-09-19 15:02:04');
INSERT INTO `sys_operationlog` VALUES (7474, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:14', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:14', 1, '2025-09-19 15:02:14');
INSERT INTO `sys_operationlog` VALUES (7475, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:27', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:27', 1, '2025-09-19 15:02:27');
INSERT INTO `sys_operationlog` VALUES (7476, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:30', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:30', 1, '2025-09-19 15:02:30');
INSERT INTO `sys_operationlog` VALUES (7477, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:31', 1, '2025-09-19 15:02:31');
INSERT INTO `sys_operationlog` VALUES (7478, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:32', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:32', 1, '2025-09-19 15:02:32');
INSERT INTO `sys_operationlog` VALUES (7479, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:32', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:32', 1, '2025-09-19 15:02:32');
INSERT INTO `sys_operationlog` VALUES (7480, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:02:58', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:02:58', 1, '2025-09-19 15:02:58');
INSERT INTO `sys_operationlog` VALUES (7481, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:05:43', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:05:43', 1, '2025-09-19 15:05:43');
INSERT INTO `sys_operationlog` VALUES (7482, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:06:19', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:06:19', 1, '2025-09-19 15:06:19');
INSERT INTO `sys_operationlog` VALUES (7483, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:06:25', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:06:25', 1, '2025-09-19 15:06:25');
INSERT INTO `sys_operationlog` VALUES (7484, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:06:26', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:06:26', 1, '2025-09-19 15:06:26');
INSERT INTO `sys_operationlog` VALUES (7485, 1, 3, '系统设置>员工管理', '添加新员工', '2025-09-19 15:06:46', '{\"User\":{\"user_id\":0,\"orgid_id\":0,\"username\":\"admin\",\"password\":\"111\",\"Salt\":null,\"name\":\"我是天才\",\"phone\":\"\",\"position\":\"管理员\",\"status\":1,\"IsDelete\":0,\"last_login_time\":null,\"user_role\":null,\"store\":null,\"RoleId\":1}}', 1, 1, '2025-09-19 15:06:46', 1, '2025-09-19 15:06:46');
INSERT INTO `sys_operationlog` VALUES (7486, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:07:26', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:07:26', 1, '2025-09-19 15:07:26');
INSERT INTO `sys_operationlog` VALUES (7487, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:07:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:07:31', 1, '2025-09-19 15:07:31');
INSERT INTO `sys_operationlog` VALUES (7488, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:07:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:07:39', 1, '2025-09-19 15:07:39');
INSERT INTO `sys_operationlog` VALUES (7489, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:08:05', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:08:05', 1, '2025-09-19 15:08:05');
INSERT INTO `sys_operationlog` VALUES (7490, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:08:11', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:08:11', 1, '2025-09-19 15:08:11');
INSERT INTO `sys_operationlog` VALUES (7491, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:08:14', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:08:14', 1, '2025-09-19 15:08:14');
INSERT INTO `sys_operationlog` VALUES (7492, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:08:20', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:08:20', 1, '2025-09-19 15:08:20');
INSERT INTO `sys_operationlog` VALUES (7493, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:08:24', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:08:24', 1, '2025-09-19 15:08:24');
INSERT INTO `sys_operationlog` VALUES (7494, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:09:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:09:31', 1, '2025-09-19 15:09:31');
INSERT INTO `sys_operationlog` VALUES (7495, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:10:00', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:10:00', 1, '2025-09-19 15:10:00');
INSERT INTO `sys_operationlog` VALUES (7496, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:10:00', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:10:00', 1, '2025-09-19 15:10:00');
INSERT INTO `sys_operationlog` VALUES (7497, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:10:01', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:10:01', 1, '2025-09-19 15:10:01');
INSERT INTO `sys_operationlog` VALUES (7498, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:10:01', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:10:01', 1, '2025-09-19 15:10:01');
INSERT INTO `sys_operationlog` VALUES (7499, 1, 3, '系统设置>员工管理', '添加新员工', '2025-09-19 15:10:21', '{\"User\":{\"user_id\":0,\"orgid_id\":0,\"username\":\"1\",\"password\":\"RSnv6Mzr/KGVVWXwHHJfCjCx831ZWBAbRevyXj0xCsQ=\",\"Salt\":\"x7ycB+O/CVRnrd/AOuR7tQ==\",\"name\":\"1\",\"phone\":\"1\",\"position\":\"\",\"status\":1,\"IsDelete\":1,\"last_login_time\":null,\"user_role\":null,\"store\":null,\"RoleId\":1}}', 1, 1, '2025-09-19 15:10:21', 1, '2025-09-19 15:10:21');
INSERT INTO `sys_operationlog` VALUES (7500, 1, 3, '系统设置>员工管理', '添加新员工', '2025-09-19 15:12:11', '{\"User\":{\"user_id\":0,\"orgid_id\":0,\"username\":\"1\",\"password\":\"DJtZULo1fHTXyNvAe8VToK0s0JQlCcMJsB11SEvoB+8=\",\"Salt\":\"XxHIWTuum7dQ67ezM+LeFA==\",\"name\":\"1\",\"phone\":\"1\",\"position\":\"\",\"status\":1,\"IsDelete\":1,\"last_login_time\":null,\"user_role\":null,\"store\":null,\"RoleId\":1}}', 1, 1, '2025-09-19 15:12:11', 1, '2025-09-19 15:12:11');
INSERT INTO `sys_operationlog` VALUES (7501, 1, 10, '系统登陆', '人员登陆', '2025-09-19 15:19:52', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 15:19:52', 1, '2025-09-19 15:19:52');
INSERT INTO `sys_operationlog` VALUES (7502, 1, 10, '系统登陆', '人员登陆', '2025-09-19 15:22:33', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 15:22:33', 1, '2025-09-19 15:22:33');
INSERT INTO `sys_operationlog` VALUES (7503, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:22:37', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:22:37', 1, '2025-09-19 15:22:37');
INSERT INTO `sys_operationlog` VALUES (7504, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:22:38', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:22:38', 1, '2025-09-19 15:22:38');
INSERT INTO `sys_operationlog` VALUES (7505, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:24:12', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:24:12', 1, '2025-09-19 15:24:12');
INSERT INTO `sys_operationlog` VALUES (7506, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:24:12', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:24:12', 1, '2025-09-19 15:24:12');
INSERT INTO `sys_operationlog` VALUES (7507, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:24:36', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:24:36', 1, '2025-09-19 15:24:36');
INSERT INTO `sys_operationlog` VALUES (7508, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:24:36', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:24:36', 1, '2025-09-19 15:24:36');
INSERT INTO `sys_operationlog` VALUES (7509, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:24:41', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:24:41', 1, '2025-09-19 15:24:41');
INSERT INTO `sys_operationlog` VALUES (7510, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:24:41', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:24:41', 1, '2025-09-19 15:24:41');
INSERT INTO `sys_operationlog` VALUES (7511, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:25:02', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:25:02', 1, '2025-09-19 15:25:02');
INSERT INTO `sys_operationlog` VALUES (7512, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:25:02', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:25:02', 1, '2025-09-19 15:25:02');
INSERT INTO `sys_operationlog` VALUES (7513, 1, 1, '系统设置>门店设置', '修改门店信息', '2025-09-19 15:25:10', '{\"sys_Store\":{\"orgid_id\":1,\"orgid_name\":\"****医院\",\"address\":\"111\",\"status\":1,\"created_at\":\"2025-09-19T15:25:09.6731542+08:00\",\"updated_at\":\"2025-09-19T15:25:09.6731548+08:00\",\"orgid_code\":null}}', 1, 1, '2025-09-19 15:25:10', 1, '2025-09-19 15:25:10');
INSERT INTO `sys_operationlog` VALUES (7514, 1, 1, '系统设置>门店设置', '修改门店信息', '2025-09-19 15:25:52', '{\"sys_Store\":{\"orgid_id\":1,\"orgid_name\":\"****医院\",\"address\":\"111\",\"status\":1,\"created_at\":\"2025-09-19T15:25:52.1681555+08:00\",\"updated_at\":\"2025-09-19T15:25:52.168156+08:00\",\"orgid_code\":null}}', 1, 1, '2025-09-19 15:25:52', 1, '2025-09-19 15:25:52');
INSERT INTO `sys_operationlog` VALUES (7515, 1, 10, '系统登陆', '人员登陆', '2025-09-19 15:26:27', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 15:26:27', 1, '2025-09-19 15:26:27');
INSERT INTO `sys_operationlog` VALUES (7516, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:26:34', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:26:34', 1, '2025-09-19 15:26:34');
INSERT INTO `sys_operationlog` VALUES (7517, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:26:34', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:26:34', 1, '2025-09-19 15:26:34');
INSERT INTO `sys_operationlog` VALUES (7518, 1, 1, '系统设置>门店设置', '修改门店信息', '2025-09-19 15:26:42', '{\"sys_Store\":{\"orgid_id\":1,\"orgid_name\":\"****医院\",\"address\":\"111\",\"status\":1,\"created_at\":\"2025-09-19T15:26:42.3065779+08:00\",\"updated_at\":\"2025-09-19T15:26:42.3065784+08:00\",\"orgid_code\":null}}', 1, 1, '2025-09-19 15:26:42', 1, '2025-09-19 15:26:42');
INSERT INTO `sys_operationlog` VALUES (7519, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:28:50', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:28:50', 1, '2025-09-19 15:28:50');
INSERT INTO `sys_operationlog` VALUES (7520, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:28:50', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:28:50', 1, '2025-09-19 15:28:50');
INSERT INTO `sys_operationlog` VALUES (7521, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:29:03', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:29:03', 1, '2025-09-19 15:29:03');
INSERT INTO `sys_operationlog` VALUES (7522, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:29:03', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:29:03', 1, '2025-09-19 15:29:03');
INSERT INTO `sys_operationlog` VALUES (7523, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:29:04', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:29:04', 1, '2025-09-19 15:29:04');
INSERT INTO `sys_operationlog` VALUES (7524, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:29:04', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:29:04', 1, '2025-09-19 15:29:04');
INSERT INTO `sys_operationlog` VALUES (7525, 1, 1, '系统设置>门店设置', '修改门店信息', '2025-09-19 15:29:11', '{\"sys_Store\":{\"orgid_id\":1,\"orgid_name\":\"****医院\",\"address\":\"111\",\"status\":1,\"created_at\":\"2025-09-19T15:29:10.9321182+08:00\",\"updated_at\":\"2025-09-19T15:29:10.9321187+08:00\",\"orgid_code\":\"Org-00001\"}}', 1, 1, '2025-09-19 15:29:11', 1, '2025-09-19 15:29:11');
INSERT INTO `sys_operationlog` VALUES (7526, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:29:11', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:29:11', 1, '2025-09-19 15:29:11');
INSERT INTO `sys_operationlog` VALUES (7527, 1, 1, '系统设置>门店设置', '修改门店信息', '2025-09-19 15:29:18', '{\"sys_Store\":{\"orgid_id\":1,\"orgid_name\":\"****医院\",\"address\":\"*****\",\"status\":1,\"created_at\":\"2025-09-19T15:29:17.9256322+08:00\",\"updated_at\":\"2025-09-19T15:29:17.9256326+08:00\",\"orgid_code\":\"Org-00001\"}}', 1, 1, '2025-09-19 15:29:18', 1, '2025-09-19 15:29:18');
INSERT INTO `sys_operationlog` VALUES (7528, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:29:18', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:29:18', 1, '2025-09-19 15:29:18');
INSERT INTO `sys_operationlog` VALUES (7529, 1, 3, '系统设置>门店设置', '新增门店', '2025-09-19 15:31:22', '{\"sys_Store\":{\"orgid_id\":null,\"orgid_name\":\"111\",\"address\":\"1111\",\"status\":1,\"created_at\":\"2025-09-19T15:31:21.5107961+08:00\",\"updated_at\":\"2025-09-19T15:31:21.5107965+08:00\",\"orgid_code\":\"Org-5\"}}', 1, 1, '2025-09-19 15:31:22', 1, '2025-09-19 15:31:22');
INSERT INTO `sys_operationlog` VALUES (7530, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:31:22', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:31:22', 1, '2025-09-19 15:31:22');
INSERT INTO `sys_operationlog` VALUES (7531, 1, 2, '系统设置>门店设置', '删除门店', '2025-09-19 15:32:06', '{\"storeIds\":[21]}', 1, 1, '2025-09-19 15:32:06', 1, '2025-09-19 15:32:06');
INSERT INTO `sys_operationlog` VALUES (7532, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:32:06', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:32:06', 1, '2025-09-19 15:32:06');
INSERT INTO `sys_operationlog` VALUES (7533, 1, 3, '系统设置>门店设置', '新增门店', '2025-09-19 15:32:59', '{\"sys_Store\":{\"orgid_id\":null,\"orgid_name\":\"111\",\"address\":\"111\",\"status\":1,\"created_at\":\"2025-09-19T15:32:59.2246151+08:00\",\"updated_at\":\"2025-09-19T15:32:59.2246155+08:00\",\"orgid_code\":\"Org-00002\"}}', 1, 1, '2025-09-19 15:32:59', 1, '2025-09-19 15:32:59');
INSERT INTO `sys_operationlog` VALUES (7534, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:32:59', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:32:59', 1, '2025-09-19 15:32:59');
INSERT INTO `sys_operationlog` VALUES (7535, 1, 2, '系统设置>门店设置', '删除门店', '2025-09-19 15:33:02', '{\"storeIds\":[22]}', 1, 1, '2025-09-19 15:33:02', 1, '2025-09-19 15:33:02');
INSERT INTO `sys_operationlog` VALUES (7536, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:33:02', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:02', 1, '2025-09-19 15:33:02');
INSERT INTO `sys_operationlog` VALUES (7537, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:33:07', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:07', 1, '2025-09-19 15:33:07');
INSERT INTO `sys_operationlog` VALUES (7538, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:33:07', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:07', 1, '2025-09-19 15:33:07');
INSERT INTO `sys_operationlog` VALUES (7539, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:33:07', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:07', 1, '2025-09-19 15:33:07');
INSERT INTO `sys_operationlog` VALUES (7540, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:08', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:08', 1, '2025-09-19 15:33:08');
INSERT INTO `sys_operationlog` VALUES (7541, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:08', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:08', 1, '2025-09-19 15:33:08');
INSERT INTO `sys_operationlog` VALUES (7542, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:08', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:08', 1, '2025-09-19 15:33:08');
INSERT INTO `sys_operationlog` VALUES (7543, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:08', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:08', 1, '2025-09-19 15:33:08');
INSERT INTO `sys_operationlog` VALUES (7544, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:33:10', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:10', 1, '2025-09-19 15:33:10');
INSERT INTO `sys_operationlog` VALUES (7545, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:33:10', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:10', 1, '2025-09-19 15:33:10');
INSERT INTO `sys_operationlog` VALUES (7546, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:33:10', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:10', 1, '2025-09-19 15:33:10');
INSERT INTO `sys_operationlog` VALUES (7547, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:33:10', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:10', 1, '2025-09-19 15:33:10');
INSERT INTO `sys_operationlog` VALUES (7548, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:33:10', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:10', 1, '2025-09-19 15:33:10');
INSERT INTO `sys_operationlog` VALUES (7549, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:33:10', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:10', 1, '2025-09-19 15:33:10');
INSERT INTO `sys_operationlog` VALUES (7550, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:33:11', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:11', 1, '2025-09-19 15:33:11');
INSERT INTO `sys_operationlog` VALUES (7551, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:33:11', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:11', 1, '2025-09-19 15:33:11');
INSERT INTO `sys_operationlog` VALUES (7552, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:33:11', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:33:11', 1, '2025-09-19 15:33:11');
INSERT INTO `sys_operationlog` VALUES (7553, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:33:11', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:33:11', 1, '2025-09-19 15:33:11');
INSERT INTO `sys_operationlog` VALUES (7554, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:33:11', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:33:11', 1, '2025-09-19 15:33:11');
INSERT INTO `sys_operationlog` VALUES (7555, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:33:11', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:33:11', 1, '2025-09-19 15:33:11');
INSERT INTO `sys_operationlog` VALUES (7556, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:33:11', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:33:11', 1, '2025-09-19 15:33:11');
INSERT INTO `sys_operationlog` VALUES (7557, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:33:12', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:33:12', 1, '2025-09-19 15:33:12');
INSERT INTO `sys_operationlog` VALUES (7558, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:33:12', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:33:12', 1, '2025-09-19 15:33:12');
INSERT INTO `sys_operationlog` VALUES (7559, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:33:12', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:33:12', 1, '2025-09-19 15:33:12');
INSERT INTO `sys_operationlog` VALUES (7560, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:33:12', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:33:12', 1, '2025-09-19 15:33:12');
INSERT INTO `sys_operationlog` VALUES (7561, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:33:12', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:33:12', 1, '2025-09-19 15:33:12');
INSERT INTO `sys_operationlog` VALUES (7562, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:33:12', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:33:12', 1, '2025-09-19 15:33:12');
INSERT INTO `sys_operationlog` VALUES (7563, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:14', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:14', 1, '2025-09-19 15:33:14');
INSERT INTO `sys_operationlog` VALUES (7564, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:14', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:14', 1, '2025-09-19 15:33:14');
INSERT INTO `sys_operationlog` VALUES (7565, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:14', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:14', 1, '2025-09-19 15:33:14');
INSERT INTO `sys_operationlog` VALUES (7566, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:14', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:14', 1, '2025-09-19 15:33:14');
INSERT INTO `sys_operationlog` VALUES (7567, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:15', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:15', 1, '2025-09-19 15:33:15');
INSERT INTO `sys_operationlog` VALUES (7568, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:33:15', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:33:15', 1, '2025-09-19 15:33:15');
INSERT INTO `sys_operationlog` VALUES (7569, 1, 3, '系统设置>员工管理', '添加新员工', '2025-09-19 15:33:48', '{\"User\":{\"user_id\":0,\"orgid_id\":0,\"username\":\"admin\",\"password\":\"\",\"Salt\":null,\"name\":\"我是天才\",\"phone\":\"111\",\"position\":\"管理员\",\"status\":1,\"IsDelete\":0,\"last_login_time\":null,\"user_role\":null,\"store\":null,\"RoleId\":1}}', 1, 1, '2025-09-19 15:33:48', 1, '2025-09-19 15:33:48');
INSERT INTO `sys_operationlog` VALUES (7570, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:34:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:34:39', 1, '2025-09-19 15:34:39');
INSERT INTO `sys_operationlog` VALUES (7571, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:34:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:34:39', 1, '2025-09-19 15:34:39');
INSERT INTO `sys_operationlog` VALUES (7572, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:34:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:34:39', 1, '2025-09-19 15:34:39');
INSERT INTO `sys_operationlog` VALUES (7573, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:34:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:34:39', 1, '2025-09-19 15:34:39');
INSERT INTO `sys_operationlog` VALUES (7574, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:34:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:34:39', 1, '2025-09-19 15:34:39');
INSERT INTO `sys_operationlog` VALUES (7575, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:34:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:34:39', 1, '2025-09-19 15:34:39');
INSERT INTO `sys_operationlog` VALUES (7576, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:35:58', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:35:58', 1, '2025-09-19 15:35:58');
INSERT INTO `sys_operationlog` VALUES (7577, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:35:58', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:35:58', 1, '2025-09-19 15:35:58');
INSERT INTO `sys_operationlog` VALUES (7578, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:35:58', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:35:58', 1, '2025-09-19 15:35:58');
INSERT INTO `sys_operationlog` VALUES (7579, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:35:59', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:35:59', 1, '2025-09-19 15:35:59');
INSERT INTO `sys_operationlog` VALUES (7580, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:35:59', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:35:59', 1, '2025-09-19 15:35:59');
INSERT INTO `sys_operationlog` VALUES (7581, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:35:59', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:35:59', 1, '2025-09-19 15:35:59');
INSERT INTO `sys_operationlog` VALUES (7582, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:31', 1, '2025-09-19 15:36:31');
INSERT INTO `sys_operationlog` VALUES (7583, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:31', 1, '2025-09-19 15:36:31');
INSERT INTO `sys_operationlog` VALUES (7584, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:31', 1, '2025-09-19 15:36:31');
INSERT INTO `sys_operationlog` VALUES (7585, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:31', 1, '2025-09-19 15:36:31');
INSERT INTO `sys_operationlog` VALUES (7586, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:31', 1, '2025-09-19 15:36:31');
INSERT INTO `sys_operationlog` VALUES (7587, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:31', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:31', 1, '2025-09-19 15:36:31');
INSERT INTO `sys_operationlog` VALUES (7588, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:35', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:35', 1, '2025-09-19 15:36:35');
INSERT INTO `sys_operationlog` VALUES (7589, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:35', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:35', 1, '2025-09-19 15:36:35');
INSERT INTO `sys_operationlog` VALUES (7590, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:35', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:35', 1, '2025-09-19 15:36:35');
INSERT INTO `sys_operationlog` VALUES (7591, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:35', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:35', 1, '2025-09-19 15:36:35');
INSERT INTO `sys_operationlog` VALUES (7592, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:35', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:35', 1, '2025-09-19 15:36:35');
INSERT INTO `sys_operationlog` VALUES (7593, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:36:35', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:36:35', 1, '2025-09-19 15:36:35');
INSERT INTO `sys_operationlog` VALUES (7594, 1, 10, '系统登陆', '人员登陆', '2025-09-19 15:38:31', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 15:38:31', 1, '2025-09-19 15:38:31');
INSERT INTO `sys_operationlog` VALUES (7595, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:37', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:37', 1, '2025-09-19 15:38:37');
INSERT INTO `sys_operationlog` VALUES (7596, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:37', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:37', 1, '2025-09-19 15:38:37');
INSERT INTO `sys_operationlog` VALUES (7597, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:42', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:42', 1, '2025-09-19 15:38:42');
INSERT INTO `sys_operationlog` VALUES (7598, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:42', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:42', 1, '2025-09-19 15:38:42');
INSERT INTO `sys_operationlog` VALUES (7599, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:42', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:42', 1, '2025-09-19 15:38:42');
INSERT INTO `sys_operationlog` VALUES (7600, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:38:44', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:44', 1, '2025-09-19 15:38:44');
INSERT INTO `sys_operationlog` VALUES (7601, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:38:44', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:44', 1, '2025-09-19 15:38:44');
INSERT INTO `sys_operationlog` VALUES (7602, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:38:44', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:44', 1, '2025-09-19 15:38:44');
INSERT INTO `sys_operationlog` VALUES (7603, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:38:44', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:44', 1, '2025-09-19 15:38:44');
INSERT INTO `sys_operationlog` VALUES (7604, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:38:46', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:38:46', 1, '2025-09-19 15:38:46');
INSERT INTO `sys_operationlog` VALUES (7605, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:38:46', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:38:46', 1, '2025-09-19 15:38:46');
INSERT INTO `sys_operationlog` VALUES (7606, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:38:46', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:38:46', 1, '2025-09-19 15:38:46');
INSERT INTO `sys_operationlog` VALUES (7607, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:38:46', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:38:46', 1, '2025-09-19 15:38:46');
INSERT INTO `sys_operationlog` VALUES (7608, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:38:46', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:38:46', 1, '2025-09-19 15:38:46');
INSERT INTO `sys_operationlog` VALUES (7609, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:38:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:38:47', 1, '2025-09-19 15:38:47');
INSERT INTO `sys_operationlog` VALUES (7610, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:38:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:38:47', 1, '2025-09-19 15:38:47');
INSERT INTO `sys_operationlog` VALUES (7611, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:38:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:38:47', 1, '2025-09-19 15:38:47');
INSERT INTO `sys_operationlog` VALUES (7612, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:38:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:38:47', 1, '2025-09-19 15:38:47');
INSERT INTO `sys_operationlog` VALUES (7613, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:38:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:38:47', 1, '2025-09-19 15:38:47');
INSERT INTO `sys_operationlog` VALUES (7614, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:38:47', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:38:47', 1, '2025-09-19 15:38:47');
INSERT INTO `sys_operationlog` VALUES (7615, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:50', 1, '2025-09-19 15:38:50');
INSERT INTO `sys_operationlog` VALUES (7616, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:50', 1, '2025-09-19 15:38:50');
INSERT INTO `sys_operationlog` VALUES (7617, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:50', 1, '2025-09-19 15:38:50');
INSERT INTO `sys_operationlog` VALUES (7618, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:50', 1, '2025-09-19 15:38:50');
INSERT INTO `sys_operationlog` VALUES (7619, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:50', 1, '2025-09-19 15:38:50');
INSERT INTO `sys_operationlog` VALUES (7620, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:38:50', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:50', 1, '2025-09-19 15:38:50');
INSERT INTO `sys_operationlog` VALUES (7621, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:51', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:51', 1, '2025-09-19 15:38:51');
INSERT INTO `sys_operationlog` VALUES (7622, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:51', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:51', 1, '2025-09-19 15:38:51');
INSERT INTO `sys_operationlog` VALUES (7623, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:51', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:51', 1, '2025-09-19 15:38:51');
INSERT INTO `sys_operationlog` VALUES (7624, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:51', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:51', 1, '2025-09-19 15:38:51');
INSERT INTO `sys_operationlog` VALUES (7625, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:51', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:51', 1, '2025-09-19 15:38:51');
INSERT INTO `sys_operationlog` VALUES (7626, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:38:51', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:38:51', 1, '2025-09-19 15:38:51');
INSERT INTO `sys_operationlog` VALUES (7627, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 15:42:28', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:42:28', 1, '2025-09-19 15:42:28');
INSERT INTO `sys_operationlog` VALUES (7628, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:42:31', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:42:31', 1, '2025-09-19 15:42:31');
INSERT INTO `sys_operationlog` VALUES (7629, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 15:42:31', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:42:31', 1, '2025-09-19 15:42:31');
INSERT INTO `sys_operationlog` VALUES (7630, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:42:31', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:42:31', 1, '2025-09-19 15:42:31');
INSERT INTO `sys_operationlog` VALUES (7631, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:42:31', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:42:31', 1, '2025-09-19 15:42:31');
INSERT INTO `sys_operationlog` VALUES (7632, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 15:42:31', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 15:42:31', 1, '2025-09-19 15:42:31');
INSERT INTO `sys_operationlog` VALUES (7633, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:42:32', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:42:32', 1, '2025-09-19 15:42:32');
INSERT INTO `sys_operationlog` VALUES (7634, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:42:32', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:42:32', 1, '2025-09-19 15:42:32');
INSERT INTO `sys_operationlog` VALUES (7635, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:42:32', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:42:32', 1, '2025-09-19 15:42:32');
INSERT INTO `sys_operationlog` VALUES (7636, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 15:42:32', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 15:42:32', 1, '2025-09-19 15:42:32');
INSERT INTO `sys_operationlog` VALUES (7637, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:42:33', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:42:33', 1, '2025-09-19 15:42:33');
INSERT INTO `sys_operationlog` VALUES (7638, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:42:33', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:42:33', 1, '2025-09-19 15:42:33');
INSERT INTO `sys_operationlog` VALUES (7639, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:42:33', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:42:33', 1, '2025-09-19 15:42:33');
INSERT INTO `sys_operationlog` VALUES (7640, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:42:33', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:42:33', 1, '2025-09-19 15:42:33');
INSERT INTO `sys_operationlog` VALUES (7641, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 15:42:33', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 15:42:33', 1, '2025-09-19 15:42:33');
INSERT INTO `sys_operationlog` VALUES (7642, 1, 10, '系统登陆', '人员登陆', '2025-09-19 15:54:01', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 15:54:01', 1, '2025-09-19 15:54:01');
INSERT INTO `sys_operationlog` VALUES (7643, 1, 11, '账号登出', '人员退出系统', '2025-09-19 16:10:45', '{}', 1, 1, '2025-09-19 16:10:45', 1, '2025-09-19 16:10:45');
INSERT INTO `sys_operationlog` VALUES (7644, 1, 10, '系统登陆', '人员登陆', '2025-09-19 16:10:49', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 16:10:49', 1, '2025-09-19 16:10:49');
INSERT INTO `sys_operationlog` VALUES (7645, 1, 10, '系统登陆', '人员登陆', '2025-09-19 16:19:57', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 16:19:57', 1, '2025-09-19 16:19:57');
INSERT INTO `sys_operationlog` VALUES (7646, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 16:20:17', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:17', 1, '2025-09-19 16:20:17');
INSERT INTO `sys_operationlog` VALUES (7647, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 16:20:17', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:17', 1, '2025-09-19 16:20:17');
INSERT INTO `sys_operationlog` VALUES (7648, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:20:26', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:26', 1, '2025-09-19 16:20:26');
INSERT INTO `sys_operationlog` VALUES (7649, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:20:26', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:26', 1, '2025-09-19 16:20:26');
INSERT INTO `sys_operationlog` VALUES (7650, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:20:26', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:26', 1, '2025-09-19 16:20:26');
INSERT INTO `sys_operationlog` VALUES (7651, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:20:28', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:28', 1, '2025-09-19 16:20:28');
INSERT INTO `sys_operationlog` VALUES (7652, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:20:29', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:29', 1, '2025-09-19 16:20:29');
INSERT INTO `sys_operationlog` VALUES (7653, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:20:29', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:29', 1, '2025-09-19 16:20:29');
INSERT INTO `sys_operationlog` VALUES (7654, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:20:29', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:20:29', 1, '2025-09-19 16:20:29');
INSERT INTO `sys_operationlog` VALUES (7655, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:20:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:20:48', 1, '2025-09-19 16:20:48');
INSERT INTO `sys_operationlog` VALUES (7656, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:20:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:20:48', 1, '2025-09-19 16:20:48');
INSERT INTO `sys_operationlog` VALUES (7657, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:20:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:20:48', 1, '2025-09-19 16:20:48');
INSERT INTO `sys_operationlog` VALUES (7658, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:20:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:20:48', 1, '2025-09-19 16:20:48');
INSERT INTO `sys_operationlog` VALUES (7659, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:20:48', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:20:48', 1, '2025-09-19 16:20:48');
INSERT INTO `sys_operationlog` VALUES (7660, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 16:20:56', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 16:20:56', 1, '2025-09-19 16:20:56');
INSERT INTO `sys_operationlog` VALUES (7661, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 16:20:56', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":1,\"Size\":10}', 1, 1, '2025-09-19 16:20:56', 1, '2025-09-19 16:20:56');
INSERT INTO `sys_operationlog` VALUES (7662, 1, 4, '系统设置>日志管理', '系统日志分页查询', '2025-09-19 16:21:15', '{\"User\":null,\"actionType\":null,\"ActionModel\":null,\"StartTime\":null,\"EndTime\":null,\"Page\":2,\"Size\":10}', 1, 1, '2025-09-19 16:21:15', 1, '2025-09-19 16:21:15');
INSERT INTO `sys_operationlog` VALUES (7663, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:23:01', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:23:01', 1, '2025-09-19 16:23:01');
INSERT INTO `sys_operationlog` VALUES (7664, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:23:01', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:23:01', 1, '2025-09-19 16:23:01');
INSERT INTO `sys_operationlog` VALUES (7665, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:01', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:01', 1, '2025-09-19 16:23:01');
INSERT INTO `sys_operationlog` VALUES (7666, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:01', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:01', 1, '2025-09-19 16:23:01');
INSERT INTO `sys_operationlog` VALUES (7667, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:01', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:01', 1, '2025-09-19 16:23:01');
INSERT INTO `sys_operationlog` VALUES (7668, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:02', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:02', 1, '2025-09-19 16:23:02');
INSERT INTO `sys_operationlog` VALUES (7669, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:02', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:02', 1, '2025-09-19 16:23:02');
INSERT INTO `sys_operationlog` VALUES (7670, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:02', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:02', 1, '2025-09-19 16:23:02');
INSERT INTO `sys_operationlog` VALUES (7671, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:02', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:02', 1, '2025-09-19 16:23:02');
INSERT INTO `sys_operationlog` VALUES (7672, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 16:23:03', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7673, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 16:23:03', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7674, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 16:23:03', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7675, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 16:23:03', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7676, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 16:23:03', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7677, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:03', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7678, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:03', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7679, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:03', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:03', 1, '2025-09-19 16:23:03');
INSERT INTO `sys_operationlog` VALUES (7680, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:04', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:04', 1, '2025-09-19 16:23:04');
INSERT INTO `sys_operationlog` VALUES (7681, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-19 16:23:04', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:04', 1, '2025-09-19 16:23:04');
INSERT INTO `sys_operationlog` VALUES (7682, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:04', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:04', 1, '2025-09-19 16:23:04');
INSERT INTO `sys_operationlog` VALUES (7683, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:04', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:04', 1, '2025-09-19 16:23:04');
INSERT INTO `sys_operationlog` VALUES (7684, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:04', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:04', 1, '2025-09-19 16:23:04');
INSERT INTO `sys_operationlog` VALUES (7685, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:04', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:04', 1, '2025-09-19 16:23:04');
INSERT INTO `sys_operationlog` VALUES (7686, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-19 16:23:04', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 16:23:04', 1, '2025-09-19 16:23:04');
INSERT INTO `sys_operationlog` VALUES (7687, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:23:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:23:05', 1, '2025-09-19 16:23:05');
INSERT INTO `sys_operationlog` VALUES (7688, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:23:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:23:05', 1, '2025-09-19 16:23:05');
INSERT INTO `sys_operationlog` VALUES (7689, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:23:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:23:05', 1, '2025-09-19 16:23:05');
INSERT INTO `sys_operationlog` VALUES (7690, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:23:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:23:05', 1, '2025-09-19 16:23:05');
INSERT INTO `sys_operationlog` VALUES (7691, 1, 4, '系统管理>定时任务', '定时任务列表查询', '2025-09-19 16:23:05', '{\"jobName\":null,\"pageIndex\":1,\"pageSize\":10}', 1, 1, '2025-09-19 16:23:05', 1, '2025-09-19 16:23:05');
INSERT INTO `sys_operationlog` VALUES (7692, 1, 11, '账号登出', '人员退出系统', '2025-09-19 16:31:14', '{}', 1, 1, '2025-09-19 16:31:14', 1, '2025-09-19 16:31:14');
INSERT INTO `sys_operationlog` VALUES (7693, 1, 10, '系统登陆', '人员登陆', '2025-09-19 16:31:22', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 16:31:22', 1, '2025-09-19 16:31:22');
INSERT INTO `sys_operationlog` VALUES (7694, 1, 10, '系统登陆', '人员登陆', '2025-09-19 17:13:04', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 17:13:04', 1, '2025-09-19 17:13:04');
INSERT INTO `sys_operationlog` VALUES (7695, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:13:10', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:13:10', 1, '2025-09-19 17:13:10');
INSERT INTO `sys_operationlog` VALUES (7696, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:13:10', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:13:10', 1, '2025-09-19 17:13:10');
INSERT INTO `sys_operationlog` VALUES (7697, 1, 1, '系统设置>员工管理', '修改员工信息', '2025-09-19 17:13:15', '{\"User\":{\"user_id\":0,\"orgid_id\":1,\"username\":\"admin1\",\"password\":\"\",\"Salt\":null,\"name\":\"我是天才\",\"phone\":\"\",\"position\":\"管理员\",\"status\":1,\"IsDelete\":0,\"last_login_time\":null,\"AvatarUrl\":null,\"user_role\":null,\"store\":null,\"RoleId\":1}}', 1, 1, '2025-09-19 17:13:15', 1, '2025-09-19 17:13:15');
INSERT INTO `sys_operationlog` VALUES (7698, 1, 1, '系统设置>员工管理', '修改员工信息', '2025-09-19 17:13:25', '{\"User\":{\"user_id\":0,\"orgid_id\":1,\"username\":\"admin\",\"password\":\"TT4kbiqRvRI5qDd6QyHSpsihFeGJ+AZ11+MZKqX8MPk=\",\"Salt\":\"g7h3nQdkqES82yQ/FCZZrA==\",\"name\":\"我是天才1\",\"phone\":\"\",\"position\":\"管理员\",\"status\":1,\"IsDelete\":1,\"last_login_time\":null,\"AvatarUrl\":null,\"user_role\":null,\"store\":null,\"RoleId\":1}}', 1, 1, '2025-09-19 17:13:25', 1, '2025-09-19 17:13:25');
INSERT INTO `sys_operationlog` VALUES (7699, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:15:45', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:15:45', 1, '2025-09-19 17:15:45');
INSERT INTO `sys_operationlog` VALUES (7700, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:16:14', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:16:14', 1, '2025-09-19 17:16:14');
INSERT INTO `sys_operationlog` VALUES (7701, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:16:25', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:16:25', 1, '2025-09-19 17:16:25');
INSERT INTO `sys_operationlog` VALUES (7702, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:19:09', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:19:09', 1, '2025-09-19 17:19:09');
INSERT INTO `sys_operationlog` VALUES (7703, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:19:11', '{}', 1, 1, '2025-09-19 17:19:11', 1, '2025-09-19 17:19:11');
INSERT INTO `sys_operationlog` VALUES (7704, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:20:08', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:20:08', 1, '2025-09-19 17:20:08');
INSERT INTO `sys_operationlog` VALUES (7705, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:20:12', '{}', 1, 1, '2025-09-19 17:20:12', 1, '2025-09-19 17:20:12');
INSERT INTO `sys_operationlog` VALUES (7706, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:20:42', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:20:42', 1, '2025-09-19 17:20:42');
INSERT INTO `sys_operationlog` VALUES (7707, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:21:04', '{}', 1, 1, '2025-09-19 17:21:04', 1, '2025-09-19 17:21:04');
INSERT INTO `sys_operationlog` VALUES (7708, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:22:39', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:22:39', 1, '2025-09-19 17:22:39');
INSERT INTO `sys_operationlog` VALUES (7709, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:22:53', '{}', 1, 1, '2025-09-19 17:22:53', 1, '2025-09-19 17:22:53');
INSERT INTO `sys_operationlog` VALUES (7710, 1, 4, '系统设置>员工管理', '用户分页查询', '2025-09-19 17:23:57', '{\"name\":null,\"username\":null,\"phone\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-19 17:23:57', 1, '2025-09-19 17:23:57');
INSERT INTO `sys_operationlog` VALUES (7711, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:24:00', '{}', 1, 1, '2025-09-19 17:24:00', 1, '2025-09-19 17:24:00');
INSERT INTO `sys_operationlog` VALUES (7712, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:24:29', '{}', 1, 1, '2025-09-19 17:24:29', 1, '2025-09-19 17:24:29');
INSERT INTO `sys_operationlog` VALUES (7713, 1, 10, '系统登陆', '人员登陆', '2025-09-19 17:25:51', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 17:25:51', 1, '2025-09-19 17:25:51');
INSERT INTO `sys_operationlog` VALUES (7714, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:26:02', '{}', 1, 1, '2025-09-19 17:26:02', 1, '2025-09-19 17:26:02');
INSERT INTO `sys_operationlog` VALUES (7715, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:26:08', '{}', 1, 1, '2025-09-19 17:26:08', 1, '2025-09-19 17:26:08');
INSERT INTO `sys_operationlog` VALUES (7716, 1, 10, '系统登陆', '人员登陆', '2025-09-19 17:27:44', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-19 17:27:44', 1, '2025-09-19 17:27:44');
INSERT INTO `sys_operationlog` VALUES (7717, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-19 17:29:15', '{}', 1, 1, '2025-09-19 17:29:15', 1, '2025-09-19 17:29:15');
INSERT INTO `sys_operationlog` VALUES (7718, 1, 10, '系统登陆', '人员登陆', '2025-09-20 11:33:16', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 11:33:16', 1, '2025-09-20 11:33:16');
INSERT INTO `sys_operationlog` VALUES (7719, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:33:26', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:33:26', 1, '2025-09-20 11:33:26');
INSERT INTO `sys_operationlog` VALUES (7720, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:33:26', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:33:26', 1, '2025-09-20 11:33:26');
INSERT INTO `sys_operationlog` VALUES (7721, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:35:30', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:35:30', 1, '2025-09-20 11:35:30');
INSERT INTO `sys_operationlog` VALUES (7722, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-20 11:35:34', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:35:34', 1, '2025-09-20 11:35:34');
INSERT INTO `sys_operationlog` VALUES (7723, 1, 4, '系统设置>门店设置', '门店设置查询', '2025-09-20 11:35:34', '{\"StoreName\":null,\"phone\":null,\"address\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:35:34', 1, '2025-09-20 11:35:34');
INSERT INTO `sys_operationlog` VALUES (7724, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:35:35', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:35:35', 1, '2025-09-20 11:35:35');
INSERT INTO `sys_operationlog` VALUES (7725, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:35:35', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:35:35', 1, '2025-09-20 11:35:35');
INSERT INTO `sys_operationlog` VALUES (7726, 1, 1, '系统设置>角色管理', '修改角色权限', '2025-09-20 11:35:38', '{\"roleId\":1,\"permissionIds\":[12,86,87,88,93,95,96]}', 1, 1, '2025-09-20 11:35:38', 1, '2025-09-20 11:35:38');
INSERT INTO `sys_operationlog` VALUES (7727, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:35:40', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:35:40', 1, '2025-09-20 11:35:40');
INSERT INTO `sys_operationlog` VALUES (7728, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:40:24', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:40:24', 1, '2025-09-20 11:40:24');
INSERT INTO `sys_operationlog` VALUES (7729, 1, 1, '系统设置>角色管理', '修改角色权限', '2025-09-20 11:40:37', '{\"roleId\":1,\"permissionIds\":[12,86,87,88,93,95,96,103]}', 1, 1, '2025-09-20 11:40:37', 1, '2025-09-20 11:40:37');
INSERT INTO `sys_operationlog` VALUES (7730, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 11:40:38', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:40:38', 1, '2025-09-20 11:40:38');
INSERT INTO `sys_operationlog` VALUES (7731, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-20 11:41:00', '{}', 1, 1, '2025-09-20 11:41:00', 1, '2025-09-20 11:41:00');
INSERT INTO `sys_operationlog` VALUES (7732, 1, 10, '系统登陆', '人员登陆', '2025-09-20 11:42:59', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 11:42:59', 1, '2025-09-20 11:42:59');
INSERT INTO `sys_operationlog` VALUES (7733, 1, 4, '患者管理', '查询患者', '2025-09-20 11:44:46', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:44:46', 1, '2025-09-20 11:44:46');
INSERT INTO `sys_operationlog` VALUES (7734, 1, 4, '患者管理', '查询患者', '2025-09-20 11:44:49', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:44:49', 1, '2025-09-20 11:44:49');
INSERT INTO `sys_operationlog` VALUES (7735, 1, 4, '患者管理', '查询患者', '2025-09-20 11:44:53', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 11:44:53', 1, '2025-09-20 11:44:53');
INSERT INTO `sys_operationlog` VALUES (7736, 1, 10, '系统登陆', '人员登陆', '2025-09-20 14:22:15', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 14:22:15', 1, '2025-09-20 14:22:15');
INSERT INTO `sys_operationlog` VALUES (7737, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 14:22:21', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:22:21', 1, '2025-09-20 14:22:21');
INSERT INTO `sys_operationlog` VALUES (7738, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 14:22:21', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:22:21', 1, '2025-09-20 14:22:21');
INSERT INTO `sys_operationlog` VALUES (7739, 1, 1, '系统设置>角色管理', '修改角色权限', '2025-09-20 14:22:27', '{\"roleId\":1,\"permissionIds\":[12,86,87,88,93,95,96,103,104]}', 1, 1, '2025-09-20 14:22:27', 1, '2025-09-20 14:22:27');
INSERT INTO `sys_operationlog` VALUES (7740, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 14:22:30', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:22:30', 1, '2025-09-20 14:22:30');
INSERT INTO `sys_operationlog` VALUES (7741, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 14:22:32', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:22:32', 1, '2025-09-20 14:22:32');
INSERT INTO `sys_operationlog` VALUES (7742, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 14:22:32', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:22:32', 1, '2025-09-20 14:22:32');
INSERT INTO `sys_operationlog` VALUES (7743, 1, 4, '系统设置>员工管理', '查看个人信息', '2025-09-20 14:23:01', '{}', 1, 1, '2025-09-20 14:23:01', 1, '2025-09-20 14:23:01');
INSERT INTO `sys_operationlog` VALUES (7744, 1, 4, '患者管理', '查询患者', '2025-09-20 14:23:37', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:37', 1, '2025-09-20 14:23:37');
INSERT INTO `sys_operationlog` VALUES (7745, 1, 4, '患者管理', '查询患者', '2025-09-20 14:23:37', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:37', 1, '2025-09-20 14:23:37');
INSERT INTO `sys_operationlog` VALUES (7746, 1, 4, '患者管理', '查询患者', '2025-09-20 14:23:37', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:37', 1, '2025-09-20 14:23:37');
INSERT INTO `sys_operationlog` VALUES (7747, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 14:23:39', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:39', 1, '2025-09-20 14:23:39');
INSERT INTO `sys_operationlog` VALUES (7748, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 14:23:39', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:39', 1, '2025-09-20 14:23:39');
INSERT INTO `sys_operationlog` VALUES (7749, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 14:23:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:40', 1, '2025-09-20 14:23:40');
INSERT INTO `sys_operationlog` VALUES (7750, 1, 4, '患者管理', '查询患者', '2025-09-20 14:23:40', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:40', 1, '2025-09-20 14:23:40');
INSERT INTO `sys_operationlog` VALUES (7751, 1, 4, '患者管理', '查询患者', '2025-09-20 14:23:40', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:40', 1, '2025-09-20 14:23:40');
INSERT INTO `sys_operationlog` VALUES (7752, 1, 4, '患者管理', '查询患者', '2025-09-20 14:23:40', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:23:40', 1, '2025-09-20 14:23:40');
INSERT INTO `sys_operationlog` VALUES (7753, 1, 10, '系统登陆', '人员登陆', '2025-09-20 14:46:36', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 14:46:36', 1, '2025-09-20 14:46:36');
INSERT INTO `sys_operationlog` VALUES (7754, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 14:46:46', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:46:46', 1, '2025-09-20 14:46:46');
INSERT INTO `sys_operationlog` VALUES (7755, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 14:46:46', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:46:46', 1, '2025-09-20 14:46:46');
INSERT INTO `sys_operationlog` VALUES (7756, 1, 1, '系统设置>角色管理', '修改角色权限', '2025-09-20 14:46:52', '{\"roleId\":1,\"permissionIds\":[12,86,87,88,93,95,96,103,104,105]}', 1, 1, '2025-09-20 14:46:52', 1, '2025-09-20 14:46:52');
INSERT INTO `sys_operationlog` VALUES (7757, 1, 4, '系统设置>角色管理', '角色管理查询', '2025-09-20 14:46:56', '{\"RoleName\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:46:56', 1, '2025-09-20 14:46:56');
INSERT INTO `sys_operationlog` VALUES (7758, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 14:47:00', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:47:00', 1, '2025-09-20 14:47:00');
INSERT INTO `sys_operationlog` VALUES (7759, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 14:47:00', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:47:00', 1, '2025-09-20 14:47:00');
INSERT INTO `sys_operationlog` VALUES (7760, 1, 4, '患者管理', '查询患者', '2025-09-20 14:47:08', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:47:08', 1, '2025-09-20 14:47:08');
INSERT INTO `sys_operationlog` VALUES (7761, 1, 4, '患者管理', '查询患者', '2025-09-20 14:47:08', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:47:08', 1, '2025-09-20 14:47:08');
INSERT INTO `sys_operationlog` VALUES (7762, 1, 4, '患者管理', '查询患者', '2025-09-20 14:47:08', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:47:08', 1, '2025-09-20 14:47:08');
INSERT INTO `sys_operationlog` VALUES (7763, 1, 3, '患者管理', '新增患者', '2025-09-20 14:47:43', '{\"patient\":{\"id\":0,\"name\":\"张三\",\"gender\":\"男\",\"age\":25,\"id_card\":\"111111111111111111\",\"contact\":\"11111111111\",\"medical_no\":\"bh1111\",\"createtime\":\"2025-09-20T14:47:42.9481609+08:00\",\"updatetime\":\"2025-09-20T14:47:42.9482325+08:00\",\"status\":1}}', 1, 1, '2025-09-20 14:47:43', 1, '2025-09-20 14:47:43');
INSERT INTO `sys_operationlog` VALUES (7764, 1, 3, '患者管理', '新增患者', '2025-09-20 14:48:30', '{\"patient\":{\"id\":0,\"name\":\"张三\",\"gender\":\"男\",\"age\":25,\"id_card\":\"111111111111111111\",\"contact\":\"11111111111\",\"medical_no\":\"bh1111\",\"createtime\":\"2025-09-20T14:48:29.8574578+08:00\",\"updatetime\":\"2025-09-20T14:48:29.8574591+08:00\",\"status\":1}}', 1, 1, '2025-09-20 14:48:30', 1, '2025-09-20 14:48:30');
INSERT INTO `sys_operationlog` VALUES (7765, 1, 3, '患者管理', '新增患者', '2025-09-20 14:51:21', '{\"patient\":{\"id\":0,\"name\":\"张三\",\"gender\":\"男\",\"age\":25,\"id_card\":\"111111111111111111\",\"contact\":\"11111111111\",\"medical_no\":\"bh1111\",\"createtime\":\"2025-09-20T14:50:29.4710374+08:00\",\"updatetime\":\"2025-09-20T14:50:30.5267225+08:00\",\"status\":1}}', 1, 1, '2025-09-20 14:51:21', 1, '2025-09-20 14:51:21');
INSERT INTO `sys_operationlog` VALUES (7766, 1, 3, '患者管理', '新增患者', '2025-09-20 14:53:22', '{\"patient\":{\"id\":0,\"name\":\"张三\",\"gender\":\"男\",\"age\":25,\"id_card\":\"111111111111111111\",\"contact\":\"11111111111\",\"medical_no\":\"bh1111\",\"createtime\":\"2025-09-20T14:53:19.2568517+08:00\",\"updatetime\":\"2025-09-20T14:53:19.5245303+08:00\",\"status\":1}}', 1, 1, '2025-09-20 14:53:22', 1, '2025-09-20 14:53:22');
INSERT INTO `sys_operationlog` VALUES (7767, 1, 4, '患者管理', '查询患者', '2025-09-20 14:53:22', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:53:22', 1, '2025-09-20 14:53:22');
INSERT INTO `sys_operationlog` VALUES (7768, 1, 1, '患者管理', '修改患者', '2025-09-20 14:53:36', '{\"patient\":{\"id\":1,\"name\":\"张三\",\"gender\":\"男\",\"age\":25,\"id_card\":\"111111111111111111\",\"contact\":\"11111111112\",\"medical_no\":\"bh1111\",\"createtime\":\"2025-09-20T14:53:19\",\"updatetime\":\"2025-09-20T14:53:36.2002839+08:00\",\"status\":1}}', 1, 1, '2025-09-20 14:53:36', 1, '2025-09-20 14:53:36');
INSERT INTO `sys_operationlog` VALUES (7769, 1, 4, '患者管理', '查询患者', '2025-09-20 14:53:36', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:53:36', 1, '2025-09-20 14:53:36');
INSERT INTO `sys_operationlog` VALUES (7770, 1, 1, '患者管理', '修改患者', '2025-09-20 14:53:40', '{\"patient\":{\"id\":1,\"name\":\"张三\",\"gender\":\"女\",\"age\":25,\"id_card\":\"111111111111111111\",\"contact\":\"11111111112\",\"medical_no\":\"bh1111\",\"createtime\":\"2025-09-20T14:53:19\",\"updatetime\":\"2025-09-20T14:53:40.3562119+08:00\",\"status\":1}}', 1, 1, '2025-09-20 14:53:40', 1, '2025-09-20 14:53:40');
INSERT INTO `sys_operationlog` VALUES (7771, 1, 4, '患者管理', '查询患者', '2025-09-20 14:53:40', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:53:40', 1, '2025-09-20 14:53:40');
INSERT INTO `sys_operationlog` VALUES (7772, 1, 2, '患者管理', '删除患者', '2025-09-20 14:53:56', '{\"ids\":[1]}', 1, 1, '2025-09-20 14:53:56', 1, '2025-09-20 14:53:56');
INSERT INTO `sys_operationlog` VALUES (7773, 1, 4, '患者管理', '查询患者', '2025-09-20 14:53:56', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:53:56', 1, '2025-09-20 14:53:56');
INSERT INTO `sys_operationlog` VALUES (7774, 1, 4, '患者管理', '查询患者', '2025-09-20 14:54:00', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 14:54:00', 1, '2025-09-20 14:54:00');
INSERT INTO `sys_operationlog` VALUES (7775, 1, 10, '系统登陆', '人员登陆', '2025-09-20 15:08:32', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 15:08:32', 1, '2025-09-20 15:08:32');
INSERT INTO `sys_operationlog` VALUES (7776, 1, 4, '患者管理', '查询患者', '2025-09-20 15:08:38', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:08:38', 1, '2025-09-20 15:08:38');
INSERT INTO `sys_operationlog` VALUES (7777, 1, 4, '患者管理', '查询患者', '2025-09-20 15:08:39', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:08:39', 1, '2025-09-20 15:08:39');
INSERT INTO `sys_operationlog` VALUES (7778, 1, 4, '患者管理', '查询患者', '2025-09-20 15:08:55', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:08:55', 1, '2025-09-20 15:08:55');
INSERT INTO `sys_operationlog` VALUES (7779, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:08:59', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:08:59', 1, '2025-09-20 15:08:59');
INSERT INTO `sys_operationlog` VALUES (7780, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:08:59', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:08:59', 1, '2025-09-20 15:08:59');
INSERT INTO `sys_operationlog` VALUES (7781, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:08:59', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:08:59', 1, '2025-09-20 15:08:59');
INSERT INTO `sys_operationlog` VALUES (7782, 1, 4, '患者管理', '查询患者', '2025-09-20 15:09:15', '{\"name\":\"张\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:09:15', 1, '2025-09-20 15:09:15');
INSERT INTO `sys_operationlog` VALUES (7783, 1, 4, '患者管理', '查询患者', '2025-09-20 15:12:38', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:12:38', 1, '2025-09-20 15:12:38');
INSERT INTO `sys_operationlog` VALUES (7784, 1, 4, '患者管理', '查询患者', '2025-09-20 15:12:38', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:12:38', 1, '2025-09-20 15:12:38');
INSERT INTO `sys_operationlog` VALUES (7785, 1, 4, '患者管理', '查询患者', '2025-09-20 15:12:38', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:12:38', 1, '2025-09-20 15:12:38');
INSERT INTO `sys_operationlog` VALUES (7786, 1, 4, '患者管理', '查询患者', '2025-09-20 15:16:59', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:16:59', 1, '2025-09-20 15:16:59');
INSERT INTO `sys_operationlog` VALUES (7787, 1, 4, '患者管理', '查询患者', '2025-09-20 15:16:59', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:16:59', 1, '2025-09-20 15:16:59');
INSERT INTO `sys_operationlog` VALUES (7788, 1, 4, '患者管理', '查询患者', '2025-09-20 15:16:59', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:16:59', 1, '2025-09-20 15:16:59');
INSERT INTO `sys_operationlog` VALUES (7789, 1, 10, '系统登陆', '人员登陆', '2025-09-20 15:17:59', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 15:17:59', 1, '2025-09-20 15:17:59');
INSERT INTO `sys_operationlog` VALUES (7790, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 15:18:04', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:18:04', 1, '2025-09-20 15:18:04');
INSERT INTO `sys_operationlog` VALUES (7791, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 15:18:04', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:18:04', 1, '2025-09-20 15:18:04');
INSERT INTO `sys_operationlog` VALUES (7792, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:18:06', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:18:06', 1, '2025-09-20 15:18:06');
INSERT INTO `sys_operationlog` VALUES (7793, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:18:06', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:18:06', 1, '2025-09-20 15:18:06');
INSERT INTO `sys_operationlog` VALUES (7794, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:18:06', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:18:06', 1, '2025-09-20 15:18:06');
INSERT INTO `sys_operationlog` VALUES (7795, 1, 4, '患者管理', '查询患者', '2025-09-20 15:18:12', '{\"name\":\"1\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:18:12', 1, '2025-09-20 15:18:12');
INSERT INTO `sys_operationlog` VALUES (7796, 1, 4, '患者管理', '查询患者', '2025-09-20 15:18:15', '{\"name\":\"张\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:18:15', 1, '2025-09-20 15:18:15');
INSERT INTO `sys_operationlog` VALUES (7797, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:19:10', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:19:10', 1, '2025-09-20 15:19:10');
INSERT INTO `sys_operationlog` VALUES (7798, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:19:10', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:19:10', 1, '2025-09-20 15:19:10');
INSERT INTO `sys_operationlog` VALUES (7799, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:19:10', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:19:10', 1, '2025-09-20 15:19:10');
INSERT INTO `sys_operationlog` VALUES (7800, 1, 4, '患者管理', '查询患者', '2025-09-20 15:19:18', '{\"name\":\"咋会给你\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:19:18', 1, '2025-09-20 15:19:18');
INSERT INTO `sys_operationlog` VALUES (7801, 1, 4, '患者管理', '查询患者', '2025-09-20 15:19:20', '{\"name\":\"张\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:19:20', 1, '2025-09-20 15:19:20');
INSERT INTO `sys_operationlog` VALUES (7802, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:21:34', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:21:34', 1, '2025-09-20 15:21:34');
INSERT INTO `sys_operationlog` VALUES (7803, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:21:34', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:21:34', 1, '2025-09-20 15:21:34');
INSERT INTO `sys_operationlog` VALUES (7804, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:21:34', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:21:34', 1, '2025-09-20 15:21:34');
INSERT INTO `sys_operationlog` VALUES (7805, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:21:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:21:40', 1, '2025-09-20 15:21:40');
INSERT INTO `sys_operationlog` VALUES (7806, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:21:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:21:40', 1, '2025-09-20 15:21:40');
INSERT INTO `sys_operationlog` VALUES (7807, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:21:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:21:40', 1, '2025-09-20 15:21:40');
INSERT INTO `sys_operationlog` VALUES (7808, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:01', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:01', 1, '2025-09-20 15:22:01');
INSERT INTO `sys_operationlog` VALUES (7809, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:01', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:01', 1, '2025-09-20 15:22:01');
INSERT INTO `sys_operationlog` VALUES (7810, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:01', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:01', 1, '2025-09-20 15:22:01');
INSERT INTO `sys_operationlog` VALUES (7811, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:16', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:16', 1, '2025-09-20 15:22:16');
INSERT INTO `sys_operationlog` VALUES (7812, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:16', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:16', 1, '2025-09-20 15:22:16');
INSERT INTO `sys_operationlog` VALUES (7813, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:16', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:16', 1, '2025-09-20 15:22:16');
INSERT INTO `sys_operationlog` VALUES (7814, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:17', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:17', 1, '2025-09-20 15:22:17');
INSERT INTO `sys_operationlog` VALUES (7815, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:17', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:17', 1, '2025-09-20 15:22:17');
INSERT INTO `sys_operationlog` VALUES (7816, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:17', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:17', 1, '2025-09-20 15:22:17');
INSERT INTO `sys_operationlog` VALUES (7817, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:27', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:27', 1, '2025-09-20 15:22:27');
INSERT INTO `sys_operationlog` VALUES (7818, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:27', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:27', 1, '2025-09-20 15:22:27');
INSERT INTO `sys_operationlog` VALUES (7819, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:27', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:27', 1, '2025-09-20 15:22:27');
INSERT INTO `sys_operationlog` VALUES (7820, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:38', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:38', 1, '2025-09-20 15:22:38');
INSERT INTO `sys_operationlog` VALUES (7821, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:38', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:38', 1, '2025-09-20 15:22:38');
INSERT INTO `sys_operationlog` VALUES (7822, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:22:38', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:22:38', 1, '2025-09-20 15:22:38');
INSERT INTO `sys_operationlog` VALUES (7823, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:23:13', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:23:13', 1, '2025-09-20 15:23:13');
INSERT INTO `sys_operationlog` VALUES (7824, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:23:13', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:23:13', 1, '2025-09-20 15:23:13');
INSERT INTO `sys_operationlog` VALUES (7825, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:23:13', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:23:13', 1, '2025-09-20 15:23:13');
INSERT INTO `sys_operationlog` VALUES (7826, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:28:45', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:28:45', 1, '2025-09-20 15:28:45');
INSERT INTO `sys_operationlog` VALUES (7827, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:28:45', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:28:45', 1, '2025-09-20 15:28:45');
INSERT INTO `sys_operationlog` VALUES (7828, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:28:45', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:28:45', 1, '2025-09-20 15:28:45');
INSERT INTO `sys_operationlog` VALUES (7829, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:28:48', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:28:48', 1, '2025-09-20 15:28:48');
INSERT INTO `sys_operationlog` VALUES (7830, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:28:48', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:28:48', 1, '2025-09-20 15:28:48');
INSERT INTO `sys_operationlog` VALUES (7831, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:28:48', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:28:48', 1, '2025-09-20 15:28:48');
INSERT INTO `sys_operationlog` VALUES (7832, 1, 4, '患者管理', '查询患者', '2025-09-20 15:32:49', '{\"name\":\"张\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:32:49', 1, '2025-09-20 15:32:49');
INSERT INTO `sys_operationlog` VALUES (7833, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:11', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:11', 1, '2025-09-20 15:36:11');
INSERT INTO `sys_operationlog` VALUES (7834, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:11', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:11', 1, '2025-09-20 15:36:11');
INSERT INTO `sys_operationlog` VALUES (7835, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:11', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:11', 1, '2025-09-20 15:36:11');
INSERT INTO `sys_operationlog` VALUES (7836, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:22', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:22', 1, '2025-09-20 15:36:22');
INSERT INTO `sys_operationlog` VALUES (7837, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:22', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:22', 1, '2025-09-20 15:36:22');
INSERT INTO `sys_operationlog` VALUES (7838, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:22', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:22', 1, '2025-09-20 15:36:22');
INSERT INTO `sys_operationlog` VALUES (7839, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:30', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:30', 1, '2025-09-20 15:36:30');
INSERT INTO `sys_operationlog` VALUES (7840, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:30', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:30', 1, '2025-09-20 15:36:30');
INSERT INTO `sys_operationlog` VALUES (7841, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:30', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:30', 1, '2025-09-20 15:36:30');
INSERT INTO `sys_operationlog` VALUES (7842, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:39', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:39', 1, '2025-09-20 15:36:39');
INSERT INTO `sys_operationlog` VALUES (7843, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:39', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:39', 1, '2025-09-20 15:36:39');
INSERT INTO `sys_operationlog` VALUES (7844, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:36:39', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:36:39', 1, '2025-09-20 15:36:39');
INSERT INTO `sys_operationlog` VALUES (7845, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:41:02', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:41:02', 1, '2025-09-20 15:41:02');
INSERT INTO `sys_operationlog` VALUES (7846, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:42:00', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:42:00', 1, '2025-09-20 15:42:00');
INSERT INTO `sys_operationlog` VALUES (7847, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:42:00', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:42:00', 1, '2025-09-20 15:42:00');
INSERT INTO `sys_operationlog` VALUES (7848, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:42:00', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:42:00', 1, '2025-09-20 15:42:00');
INSERT INTO `sys_operationlog` VALUES (7849, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:42:54', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:42:54', 1, '2025-09-20 15:42:54');
INSERT INTO `sys_operationlog` VALUES (7850, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:42:54', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:42:54', 1, '2025-09-20 15:42:54');
INSERT INTO `sys_operationlog` VALUES (7851, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:42:54', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:42:54', 1, '2025-09-20 15:42:54');
INSERT INTO `sys_operationlog` VALUES (7852, 1, 10, '系统登陆', '人员登陆', '2025-09-20 15:46:38', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 15:46:38', 1, '2025-09-20 15:46:38');
INSERT INTO `sys_operationlog` VALUES (7853, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:46:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:46:40', 1, '2025-09-20 15:46:40');
INSERT INTO `sys_operationlog` VALUES (7854, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 15:46:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:46:40', 1, '2025-09-20 15:46:40');
INSERT INTO `sys_operationlog` VALUES (7855, 1, 4, '患者管理', '查询患者', '2025-09-20 15:46:45', '{\"name\":\"咋会给你\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:46:45', 1, '2025-09-20 15:46:45');
INSERT INTO `sys_operationlog` VALUES (7856, 1, 4, '患者管理', '查询患者', '2025-09-20 15:46:46', '{\"name\":\"咋会给\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:46:46', 1, '2025-09-20 15:46:46');
INSERT INTO `sys_operationlog` VALUES (7857, 1, 4, '患者管理', '查询患者', '2025-09-20 15:46:50', '{\"name\":\"张\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 15:46:50', 1, '2025-09-20 15:46:50');
INSERT INTO `sys_operationlog` VALUES (7858, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 15:54:52', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:54:52', 1, '2025-09-20 15:54:52');
INSERT INTO `sys_operationlog` VALUES (7859, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 15:54:52', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:54:52', 1, '2025-09-20 15:54:52');
INSERT INTO `sys_operationlog` VALUES (7860, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 15:54:52', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 15:54:52', 1, '2025-09-20 15:54:52');
INSERT INTO `sys_operationlog` VALUES (7861, 1, 10, '系统登陆', '人员登陆', '2025-09-20 16:08:08', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 16:08:08', 1, '2025-09-20 16:08:08');
INSERT INTO `sys_operationlog` VALUES (7862, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:08:56', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:08:56', 1, '2025-09-20 16:08:56');
INSERT INTO `sys_operationlog` VALUES (7863, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:08:56', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:08:56', 1, '2025-09-20 16:08:56');
INSERT INTO `sys_operationlog` VALUES (7864, 1, 4, '患者管理', '查询患者', '2025-09-20 16:09:05', '{\"name\":\"张\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 16:09:05', 1, '2025-09-20 16:09:05');
INSERT INTO `sys_operationlog` VALUES (7865, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:11:04', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:11:04', 1, '2025-09-20 16:11:04');
INSERT INTO `sys_operationlog` VALUES (7866, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:13:05', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:13:05', 1, '2025-09-20 16:13:05');
INSERT INTO `sys_operationlog` VALUES (7867, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:13:05', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:13:05', 1, '2025-09-20 16:13:05');
INSERT INTO `sys_operationlog` VALUES (7868, 1, 10, '系统登陆', '人员登陆', '2025-09-20 16:16:27', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 16:16:27', 1, '2025-09-20 16:16:27');
INSERT INTO `sys_operationlog` VALUES (7869, 1, 4, '患者管理', '查询患者', '2025-09-20 16:16:32', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:16:32', 1, '2025-09-20 16:16:32');
INSERT INTO `sys_operationlog` VALUES (7870, 1, 4, '患者管理', '查询患者', '2025-09-20 16:16:32', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:16:32', 1, '2025-09-20 16:16:32');
INSERT INTO `sys_operationlog` VALUES (7871, 1, 4, '患者管理', '查询患者', '2025-09-20 16:18:09', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:18:09', 1, '2025-09-20 16:18:09');
INSERT INTO `sys_operationlog` VALUES (7872, 1, 4, '患者管理', '查询患者', '2025-09-20 16:18:09', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:18:09', 1, '2025-09-20 16:18:09');
INSERT INTO `sys_operationlog` VALUES (7873, 1, 3, '患者管理', '新增患者', '2025-09-20 16:18:27', '{\"patient\":{\"id\":0,\"name\":\"李四\",\"gender\":\"男\",\"age\":26,\"id_card\":\"111\",\"contact\":\"44\",\"medical_no\":\"44\",\"createtime\":\"2025-09-20T16:18:27.0574759+08:00\",\"updatetime\":\"2025-09-20T16:18:27.0574772+08:00\",\"status\":1}}', 1, 1, '2025-09-20 16:18:27', 1, '2025-09-20 16:18:27');
INSERT INTO `sys_operationlog` VALUES (7874, 1, 4, '患者管理', '查询患者', '2025-09-20 16:18:27', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:18:27', 1, '2025-09-20 16:18:27');
INSERT INTO `sys_operationlog` VALUES (7875, 1, 10, '系统登陆', '人员登陆', '2025-09-20 16:20:41', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 16:20:41', 1, '2025-09-20 16:20:41');
INSERT INTO `sys_operationlog` VALUES (7876, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:20:46', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:20:46', 1, '2025-09-20 16:20:46');
INSERT INTO `sys_operationlog` VALUES (7877, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:20:46', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:20:46', 1, '2025-09-20 16:20:46');
INSERT INTO `sys_operationlog` VALUES (7878, 1, 4, '患者管理', '查询患者', '2025-09-20 16:20:51', '{\"name\":\"1\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 16:20:51', 1, '2025-09-20 16:20:51');
INSERT INTO `sys_operationlog` VALUES (7879, 1, 4, '患者管理', '查询患者', '2025-09-20 16:20:54', '{\"name\":\"李四\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 16:20:54', 1, '2025-09-20 16:20:54');
INSERT INTO `sys_operationlog` VALUES (7880, 1, 10, '系统登陆', '人员登陆', '2025-09-20 16:26:37', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 16:26:37', 1, '2025-09-20 16:26:37');
INSERT INTO `sys_operationlog` VALUES (7881, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:26:44', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:26:44', 1, '2025-09-20 16:26:44');
INSERT INTO `sys_operationlog` VALUES (7882, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:26:44', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:26:44', 1, '2025-09-20 16:26:44');
INSERT INTO `sys_operationlog` VALUES (7883, 1, 4, '患者管理', '查询患者', '2025-09-20 16:26:52', '{\"name\":\"李四\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 16:26:52', 1, '2025-09-20 16:26:52');
INSERT INTO `sys_operationlog` VALUES (7884, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:28:02', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:28:02', 1, '2025-09-20 16:28:02');
INSERT INTO `sys_operationlog` VALUES (7885, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:28:02', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:28:02', 1, '2025-09-20 16:28:02');
INSERT INTO `sys_operationlog` VALUES (7886, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:28:09', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:28:09', 1, '2025-09-20 16:28:09');
INSERT INTO `sys_operationlog` VALUES (7887, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:28:10', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:28:10', 1, '2025-09-20 16:28:10');
INSERT INTO `sys_operationlog` VALUES (7888, 1, 4, '患者管理', '查询患者', '2025-09-20 16:28:49', '{\"name\":\"李四\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 16:28:49', 1, '2025-09-20 16:28:49');
INSERT INTO `sys_operationlog` VALUES (7889, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:30:33', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:30:33', 1, '2025-09-20 16:30:33');
INSERT INTO `sys_operationlog` VALUES (7890, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:30:33', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:30:33', 1, '2025-09-20 16:30:33');
INSERT INTO `sys_operationlog` VALUES (7891, 1, 10, '系统登陆', '人员登陆', '2025-09-20 16:32:24', '账号：admin,员工姓名：我是天才', 1, 1, '2025-09-20 16:32:24', 1, '2025-09-20 16:32:24');
INSERT INTO `sys_operationlog` VALUES (7892, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:32:26', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:26', 1, '2025-09-20 16:32:26');
INSERT INTO `sys_operationlog` VALUES (7893, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:32:26', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:26', 1, '2025-09-20 16:32:26');
INSERT INTO `sys_operationlog` VALUES (7894, 1, 4, '患者管理', '查询患者', '2025-09-20 16:32:32', '{\"name\":\"李四\",\"medicalNo\":null,\"page\":1,\"size\":20}', 1, 1, '2025-09-20 16:32:32', 1, '2025-09-20 16:32:32');
INSERT INTO `sys_operationlog` VALUES (7895, 1, 3, '检查管理', '新增检查数据', '2025-09-20 16:32:39', '{\"exam\":{\"id\":0,\"exam_no\":\"1\",\"patient_id\":2,\"org_id\":1,\"exam_type\":\"CT\",\"exam_date\":\"2025-09-20T00:32:27Z\",\"report_path\":\"啊\",\"image_path\":\"1\",\"status\":1,\"create_time\":\"2025-09-20T16:32:38.623305+08:00\",\"update_time\":\"2025-09-20T16:32:38.623368+08:00\",\"is_printed\":0,\"patient\":{\"id\":2,\"name\":\"李四\",\"gender\":\"男\",\"age\":26,\"id_card\":\"111\",\"contact\":\"44\",\"medical_no\":\"44\",\"createtime\":\"2025-09-20T16:18:27\",\"updatetime\":\"2025-09-20T16:18:27\",\"status\":1}}}', 1, 1, '2025-09-20 16:32:39', 1, '2025-09-20 16:32:39');
INSERT INTO `sys_operationlog` VALUES (7896, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:32:39', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:39', 1, '2025-09-20 16:32:39');
INSERT INTO `sys_operationlog` VALUES (7897, 1, 16, '检查管理', '打印报告', '2025-09-20 16:32:49', '{\"examId\":1}', 1, 1, '2025-09-20 16:32:49', 1, '2025-09-20 16:32:49');
INSERT INTO `sys_operationlog` VALUES (7898, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:32:49', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:49', 1, '2025-09-20 16:32:49');
INSERT INTO `sys_operationlog` VALUES (7899, 1, 1, '检查管理', '解锁打印', '2025-09-20 16:32:51', '{\"examId\":1}', 1, 1, '2025-09-20 16:32:51', 1, '2025-09-20 16:32:51');
INSERT INTO `sys_operationlog` VALUES (7900, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:32:51', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:51', 1, '2025-09-20 16:32:51');
INSERT INTO `sys_operationlog` VALUES (7901, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:32:54', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:54', 1, '2025-09-20 16:32:54');
INSERT INTO `sys_operationlog` VALUES (7902, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:32:54', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:54', 1, '2025-09-20 16:32:54');
INSERT INTO `sys_operationlog` VALUES (7903, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:32:54', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:32:54', 1, '2025-09-20 16:32:54');
INSERT INTO `sys_operationlog` VALUES (7904, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:36:55', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:36:55', 1, '2025-09-20 16:36:55');
INSERT INTO `sys_operationlog` VALUES (7905, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:36:57', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:36:57', 1, '2025-09-20 16:36:57');
INSERT INTO `sys_operationlog` VALUES (7906, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:37:04', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:37:04', 1, '2025-09-20 16:37:04');
INSERT INTO `sys_operationlog` VALUES (7907, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:37:04', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:37:04', 1, '2025-09-20 16:37:04');
INSERT INTO `sys_operationlog` VALUES (7908, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:37:04', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:37:04', 1, '2025-09-20 16:37:04');
INSERT INTO `sys_operationlog` VALUES (7909, 1, 16, '检查管理', '打印报告', '2025-09-20 16:37:05', '{\"examId\":1}', 1, 1, '2025-09-20 16:37:05', 1, '2025-09-20 16:37:05');
INSERT INTO `sys_operationlog` VALUES (7910, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:37:05', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:37:05', 1, '2025-09-20 16:37:05');
INSERT INTO `sys_operationlog` VALUES (7911, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:37:06', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:37:06', 1, '2025-09-20 16:37:06');
INSERT INTO `sys_operationlog` VALUES (7912, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:37:06', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:37:06', 1, '2025-09-20 16:37:06');
INSERT INTO `sys_operationlog` VALUES (7913, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:37:06', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:37:06', 1, '2025-09-20 16:37:06');
INSERT INTO `sys_operationlog` VALUES (7914, 1, 2, '打印记录管理', '删除打印记录', '2025-09-20 16:39:42', '{\"ids\":[2]}', 1, 1, '2025-09-20 16:39:42', 1, '2025-09-20 16:39:42');
INSERT INTO `sys_operationlog` VALUES (7915, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:39:42', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:42', 1, '2025-09-20 16:39:42');
INSERT INTO `sys_operationlog` VALUES (7916, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:45', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:45', 1, '2025-09-20 16:39:45');
INSERT INTO `sys_operationlog` VALUES (7917, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:45', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:45', 1, '2025-09-20 16:39:45');
INSERT INTO `sys_operationlog` VALUES (7918, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:45', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:45', 1, '2025-09-20 16:39:45');
INSERT INTO `sys_operationlog` VALUES (7919, 1, 1, '检查管理', '解锁打印', '2025-09-20 16:39:47', '{\"examId\":1}', 1, 1, '2025-09-20 16:39:47', 1, '2025-09-20 16:39:47');
INSERT INTO `sys_operationlog` VALUES (7920, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:47', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:47', 1, '2025-09-20 16:39:47');
INSERT INTO `sys_operationlog` VALUES (7921, 1, 16, '检查管理', '打印报告', '2025-09-20 16:39:49', '{\"examId\":1}', 1, 1, '2025-09-20 16:39:49', 1, '2025-09-20 16:39:49');
INSERT INTO `sys_operationlog` VALUES (7922, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:49', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:49', 1, '2025-09-20 16:39:49');
INSERT INTO `sys_operationlog` VALUES (7923, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:39:50', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:50', 1, '2025-09-20 16:39:50');
INSERT INTO `sys_operationlog` VALUES (7924, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:39:50', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:50', 1, '2025-09-20 16:39:50');
INSERT INTO `sys_operationlog` VALUES (7925, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:39:50', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:50', 1, '2025-09-20 16:39:50');
INSERT INTO `sys_operationlog` VALUES (7926, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:52', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:52', 1, '2025-09-20 16:39:52');
INSERT INTO `sys_operationlog` VALUES (7927, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:52', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:52', 1, '2025-09-20 16:39:52');
INSERT INTO `sys_operationlog` VALUES (7928, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:52', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:52', 1, '2025-09-20 16:39:52');
INSERT INTO `sys_operationlog` VALUES (7929, 1, 4, '患者管理', '查询患者', '2025-09-20 16:39:52', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:52', 1, '2025-09-20 16:39:52');
INSERT INTO `sys_operationlog` VALUES (7930, 1, 4, '患者管理', '查询患者', '2025-09-20 16:39:52', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:52', 1, '2025-09-20 16:39:52');
INSERT INTO `sys_operationlog` VALUES (7931, 1, 4, '患者管理', '查询患者', '2025-09-20 16:39:52', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:52', 1, '2025-09-20 16:39:52');
INSERT INTO `sys_operationlog` VALUES (7932, 1, 4, '患者管理', '查询患者', '2025-09-20 16:39:52', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:52', 1, '2025-09-20 16:39:52');
INSERT INTO `sys_operationlog` VALUES (7933, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:53', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:53', 1, '2025-09-20 16:39:53');
INSERT INTO `sys_operationlog` VALUES (7934, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:54', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:54', 1, '2025-09-20 16:39:54');
INSERT INTO `sys_operationlog` VALUES (7935, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:54', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:54', 1, '2025-09-20 16:39:54');
INSERT INTO `sys_operationlog` VALUES (7936, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:39:54', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:39:54', 1, '2025-09-20 16:39:54');
INSERT INTO `sys_operationlog` VALUES (7937, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:39', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7938, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:39', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7939, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:39', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7940, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:39', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7941, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:44:39', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7942, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:44:39', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7943, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:44:39', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7944, 1, 4, '打印记录管理', '分页查询打印记录', '2025-09-20 16:44:39', '{\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:39', 1, '2025-09-20 16:44:39');
INSERT INTO `sys_operationlog` VALUES (7945, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:40', 1, '2025-09-20 16:44:40');
INSERT INTO `sys_operationlog` VALUES (7946, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:40', 1, '2025-09-20 16:44:40');
INSERT INTO `sys_operationlog` VALUES (7947, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:40', 1, '2025-09-20 16:44:40');
INSERT INTO `sys_operationlog` VALUES (7948, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:40', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:40', 1, '2025-09-20 16:44:40');
INSERT INTO `sys_operationlog` VALUES (7949, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:40', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:40', 1, '2025-09-20 16:44:40');
INSERT INTO `sys_operationlog` VALUES (7950, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:40', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:40', 1, '2025-09-20 16:44:40');
INSERT INTO `sys_operationlog` VALUES (7951, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:40', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:40', 1, '2025-09-20 16:44:40');
INSERT INTO `sys_operationlog` VALUES (7952, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:41', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:41', 1, '2025-09-20 16:44:41');
INSERT INTO `sys_operationlog` VALUES (7953, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:42', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:42', 1, '2025-09-20 16:44:42');
INSERT INTO `sys_operationlog` VALUES (7954, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:42', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:42', 1, '2025-09-20 16:44:42');
INSERT INTO `sys_operationlog` VALUES (7955, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:43', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:43', 1, '2025-09-20 16:44:43');
INSERT INTO `sys_operationlog` VALUES (7956, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:43', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:43', 1, '2025-09-20 16:44:43');
INSERT INTO `sys_operationlog` VALUES (7957, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:43', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:43', 1, '2025-09-20 16:44:43');
INSERT INTO `sys_operationlog` VALUES (7958, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:43', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:43', 1, '2025-09-20 16:44:43');
INSERT INTO `sys_operationlog` VALUES (7959, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:43', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:43', 1, '2025-09-20 16:44:43');
INSERT INTO `sys_operationlog` VALUES (7960, 1, 4, '患者管理', '查询患者', '2025-09-20 16:44:43', '{\"name\":null,\"medicalNo\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:43', 1, '2025-09-20 16:44:43');
INSERT INTO `sys_operationlog` VALUES (7961, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:44', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:44', 1, '2025-09-20 16:44:44');
INSERT INTO `sys_operationlog` VALUES (7962, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:44', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:44', 1, '2025-09-20 16:44:44');
INSERT INTO `sys_operationlog` VALUES (7963, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:44', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:44', 1, '2025-09-20 16:44:44');
INSERT INTO `sys_operationlog` VALUES (7964, 1, 4, '检查管理', '分页查询检查数据', '2025-09-20 16:44:44', '{\"examNo\":null,\"patientName\":null,\"examDate\":null,\"page\":1,\"size\":10}', 1, 1, '2025-09-20 16:44:44', 1, '2025-09-20 16:44:44');

-- ----------------------------
-- Table structure for sys_orgid
-- ----------------------------
DROP TABLE IF EXISTS `sys_orgid`;
CREATE TABLE `sys_orgid`  (
  `orgid_id` bigint NOT NULL AUTO_INCREMENT COMMENT '组织ID（主键）',
  `orgid_name` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '组织名称',
  `address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '地址',
  `status` tinyint NOT NULL DEFAULT 1 COMMENT '状态（1-启用；0-停用）',
  `created_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
  `updated_at` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间',
  `orgid_code` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '组织编码',
  PRIMARY KEY (`orgid_id`) USING BTREE,
  INDEX `idx_orgidstore_status`(`status` ASC) USING BTREE,
  INDEX `idx_orgid_name`(`orgid_name` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 23 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '组织信息表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_orgid
-- ----------------------------
INSERT INTO `sys_orgid` VALUES (1, '****医院', '*****', 1, '2025-09-19 15:29:18', '2025-09-19 15:29:18', 'Org-00001');

-- ----------------------------
-- Table structure for sys_permission
-- ----------------------------
DROP TABLE IF EXISTS `sys_permission`;
CREATE TABLE `sys_permission`  (
  `permission_id` bigint NOT NULL AUTO_INCREMENT COMMENT '权限ID（主键）',
  `permission_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '权限名称（如\"订单管理\"）',
  `permission_router` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '权限路由',
  `permission_key` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '权限标识（如\"order:manage\"）',
  `parent_id` bigint NOT NULL DEFAULT 0 COMMENT '父权限ID（用于层级）',
  `permission_icon` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '图标',
  PRIMARY KEY (`permission_id`) USING BTREE,
  UNIQUE INDEX `uk_permission_key`(`permission_key` ASC) USING BTREE,
  INDEX `idx_permission_parent`(`parent_id` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 104 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '权限表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_permission
-- ----------------------------
INSERT INTO `sys_permission` VALUES (12, '系统设置', NULL, 'systemSetting', 0, '/src/assets/系统设置.png');
INSERT INTO `sys_permission` VALUES (86, '员工管理', 'StaffManagement', 'staff-management', 12, '/src/assets/员工管理.png');
INSERT INTO `sys_permission` VALUES (87, '角色权限', 'RolePermission', 'role-permission', 12, '/src/assets/角色权限.png');
INSERT INTO `sys_permission` VALUES (88, '组织设置', 'OrgSetting', 'store-setting', 12, '/src/assets/门店设置.png');
INSERT INTO `sys_permission` VALUES (93, '定时任务管理', 'TaskManagement', 'TaskManagement', 12, '/src/assets/套餐管理.png');
INSERT INTO `sys_permission` VALUES (95, '操作日志', 'OperationLogManagement', 'OperationLogManagement', 12, '/src/assets/损耗记录.png');
INSERT INTO `sys_permission` VALUES (96, '患者管理', '1', '1', 0, '/src/assets/损耗记录.png');
INSERT INTO `sys_permission` VALUES (103, '患者信息', 'PatientManagement', 'patient_Management', 96, '/src/assets/损耗记录.png');
INSERT INTO `sys_permission` VALUES (104, '诊断数据', 'ExaminationManagement', 'ExaminationManagement', 96, '/src/assets/损耗记录.png');
INSERT INTO `sys_permission` VALUES (105, '打印记录', 'PrintRecordManagement', 'PrintRecordManagement', 96, '/src/assets/损耗记录.png');

-- ----------------------------
-- Table structure for sys_role
-- ----------------------------
DROP TABLE IF EXISTS `sys_role`;
CREATE TABLE `sys_role`  (
  `role_id` bigint NOT NULL AUTO_INCREMENT COMMENT '角色ID（主键）',
  `role_name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '角色名称（店长/收银员/厨师）',
  `description` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '角色描述',
  PRIMARY KEY (`role_id`) USING BTREE,
  UNIQUE INDEX `uk_role_name`(`role_name` ASC) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 4 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_role
-- ----------------------------
INSERT INTO `sys_role` VALUES (1, '管理员', '操纵整个系统');

-- ----------------------------
-- Table structure for sys_role_permission
-- ----------------------------
DROP TABLE IF EXISTS `sys_role_permission`;
CREATE TABLE `sys_role_permission`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '关联ID（主键）',
  `role_id` bigint NOT NULL COMMENT '角色ID',
  `permission_id` bigint NOT NULL COMMENT '权限ID',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_role_permission_role`(`role_id` ASC) USING BTREE,
  INDEX `idx_role_permission_permission`(`permission_id` ASC) USING BTREE,
  CONSTRAINT `fk_role_permission_permission` FOREIGN KEY (`permission_id`) REFERENCES `sys_permission` (`permission_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_role_permission_role` FOREIGN KEY (`role_id`) REFERENCES `sys_role` (`role_id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 1601 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '角色权限关联表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_role_permission
-- ----------------------------
INSERT INTO `sys_role_permission` VALUES (1610, 1, 12);
INSERT INTO `sys_role_permission` VALUES (1611, 1, 86);
INSERT INTO `sys_role_permission` VALUES (1612, 1, 87);
INSERT INTO `sys_role_permission` VALUES (1613, 1, 88);
INSERT INTO `sys_role_permission` VALUES (1614, 1, 93);
INSERT INTO `sys_role_permission` VALUES (1615, 1, 95);
INSERT INTO `sys_role_permission` VALUES (1616, 1, 96);
INSERT INTO `sys_role_permission` VALUES (1617, 1, 103);
INSERT INTO `sys_role_permission` VALUES (1618, 1, 104);
INSERT INTO `sys_role_permission` VALUES (1619, 1, 105);

-- ----------------------------
-- Table structure for sys_timertask
-- ----------------------------
DROP TABLE IF EXISTS `sys_timertask`;
CREATE TABLE `sys_timertask`  (
  `Id` bigint NOT NULL AUTO_INCREMENT,
  `TimerName` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '定时器名称',
  `TimerClass` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '定时器服务类',
  `CreateTime` datetime NULL DEFAULT NULL COMMENT '创建时间',
  `BeginTime` datetime NULL DEFAULT NULL COMMENT '运行开始时间',
  `EndTime` datetime NULL DEFAULT NULL COMMENT '运行结束时间',
  `AddUser` bigint NULL DEFAULT NULL COMMENT '创建人',
  `OrgId` bigint NULL DEFAULT NULL COMMENT '组织',
  `IsStart` int NULL DEFAULT NULL COMMENT '运行状态：0，未启动，1，启动运行，2，暂停',
  `isDelete` int NULL DEFAULT NULL COMMENT '是否删除：0，删除，1，运行',
  `Corn` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '设置运行时段',
  `StartNumber` bigint(20) UNSIGNED ZEROFILL NOT NULL COMMENT '运行次数',
  `lastRunTime` datetime NULL DEFAULT NULL COMMENT '最后运行时间',
  PRIMARY KEY (`Id`) USING BTREE
) ENGINE = InnoDB AUTO_INCREMENT = 8 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '定时器管理' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_timertask
-- ----------------------------
INSERT INTO `sys_timertask` VALUES (4, '定时清理操作日志保留一个月', 'ClearLogsTask', '2025-09-16 14:07:09', '2025-09-16 14:07:09', '2125-09-16 14:07:09', NULL, NULL, 1, 1, '0 0 1 * * ?', 00000000000000000001, '2025-09-20 01:00:00');

-- ----------------------------
-- Table structure for sys_user
-- ----------------------------
DROP TABLE IF EXISTS `sys_user`;
CREATE TABLE `sys_user`  (
  `user_id` bigint NOT NULL AUTO_INCREMENT COMMENT '员工ID（主键）',
  `orgid_id` bigint NOT NULL,
  `username` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '登录账号',
  `password` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '加密密码',
  `name` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '姓名',
  `phone` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL COMMENT '手机号',
  `position` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL COMMENT '职位',
  `status` tinyint NOT NULL DEFAULT 1 COMMENT '状态（1-在职；0-离职）',
  `last_login_time` datetime NULL DEFAULT NULL COMMENT '最后登录时间',
  `Salt` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IsDelete` tinyint NOT NULL DEFAULT 1,
  `AvatarUrl` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NULL DEFAULT NULL,
  PRIMARY KEY (`user_id`) USING BTREE,
  UNIQUE INDEX `uk_user_username`(`username` ASC) USING BTREE,
  INDEX `idx_user_store`(`orgid_id` ASC) USING BTREE,
  INDEX `idx_user_position`(`position` ASC) USING BTREE,
  INDEX `idx_user_status`(`status` ASC) USING BTREE,
  CONSTRAINT `fk_user_store` FOREIGN KEY (`orgid_id`) REFERENCES `sys_orgid` (`orgid_id`) ON DELETE RESTRICT ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '员工表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_user
-- ----------------------------
INSERT INTO `sys_user` VALUES (1, 1, 'admin', '8pgVn7BnedAGOKthCSdIUifitpQsAWfKSDDFoz5R8oM=', '我是天才', '', '管理员', 1, '2025-09-19 16:31:14', 'SOMMsmOVbHI38x1hUkWfiQ==', 1, NULL);

-- ----------------------------
-- Table structure for sys_user_role
-- ----------------------------
DROP TABLE IF EXISTS `sys_user_role`;
CREATE TABLE `sys_user_role`  (
  `id` bigint NOT NULL AUTO_INCREMENT COMMENT '关联ID（主键）',
  `user_id` bigint NOT NULL COMMENT '员工ID',
  `role_id` bigint NOT NULL COMMENT '角色ID',
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `idx_user_role_user`(`user_id` ASC) USING BTREE,
  INDEX `idx_user_role_role`(`role_id` ASC) USING BTREE,
  CONSTRAINT `fk_user_role_role` FOREIGN KEY (`role_id`) REFERENCES `sys_role` (`role_id`) ON DELETE RESTRICT ON UPDATE RESTRICT,
  CONSTRAINT `fk_user_role_user` FOREIGN KEY (`user_id`) REFERENCES `sys_user` (`user_id`) ON DELETE CASCADE ON UPDATE RESTRICT
) ENGINE = InnoDB AUTO_INCREMENT = 10 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci COMMENT = '员工角色关联表' ROW_FORMAT = DYNAMIC;

-- ----------------------------
-- Records of sys_user_role
-- ----------------------------
INSERT INTO `sys_user_role` VALUES (1, 1, 1);

SET FOREIGN_KEY_CHECKS = 1;
