# 动态数据库连接管理服务

## 📋 功能概述

动态数据库连接管理服务允许在项目启动后动态获取数据库连接配置，并实例化数据库连接。支持从配置文件或主数据库获取连接配置。

## 🚀 主要功能

- ✅ **动态连接管理** - 运行时获取和创建数据库连接
- ✅ **多数据源支持** - 支持MySQL、SQLServer、Oracle等多种数据库
- ✅ **连接池管理** - 自动管理连接池，避免重复创建
- ✅ **连接测试** - 支持测试数据库连接可用性
- ✅ **配置管理** - 支持增删改查连接配置
- ✅ **自动刷新** - 支持动态刷新连接配置

## 🏗️ 架构设计

### 核心组件

1. **`DynamicDatabaseService`** - 动态数据库连接管理服务
2. **`DatabaseConnectionConfig`** - 数据库连接配置模型
3. **`DatabaseController`** - 数据库连接管理API控制器

### 数据流程

```
项目启动 → 加载主数据库连接 → 获取外部连接配置 → 创建连接池 → 提供服务
```

## 📦 配置说明

### 1. 配置文件方式

在 `appsettings.json` 中配置：

```json
{
  "DatabaseConnections": {
    "ExternalDB1": {
      "ConnectionString": "Server=192.168.1.100;Database=ExternalDB1;User Id=user;Password=pass;",
      "DatabaseType": "1",
      "IsEnabled": true,
      "Description": "外部数据库1"
    },
    "ExternalDB2": {
      "ConnectionString": "Server=192.168.1.101;Database=ExternalDB2;User Id=user;Password=pass;",
      "DatabaseType": "0",
      "IsEnabled": true,
      "Description": "外部数据库2"
    }
  }
}
```

### 2. 数据库配置方式

在主数据库中创建 `database_connection_config` 表：

```sql
CREATE TABLE database_connection_config (
    id INT PRIMARY KEY AUTO_INCREMENT,
    name VARCHAR(100) NOT NULL UNIQUE,
    connection_string TEXT NOT NULL,
    database_type INT NOT NULL,
    is_enabled BOOLEAN DEFAULT TRUE,
    description VARCHAR(500),
    create_time DATETIME DEFAULT CURRENT_TIMESTAMP,
    update_time DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    created_by VARCHAR(100),
    updated_by VARCHAR(100)
);
```

### 3. 数据库类型枚举

```csharp
public enum DbType
{
    MySql = 0,      // MySQL
    SqlServer = 1,  // SQL Server
    Sqlite = 2,     // SQLite
    Oracle = 3,     // Oracle
    PostgreSQL = 4  // PostgreSQL
}
```

## 📡 API接口

### 1. 获取可用连接
```http
GET /api/Database/connections
```

### 2. 测试数据库连接
```http
POST /api/Database/test?connectionName=ExternalDB1
```

### 3. 刷新连接配置
```http
POST /api/Database/refresh
```

### 4. 获取连接配置列表
```http
GET /api/Database/configs
```

### 5. 添加连接配置
```http
POST /api/Database/config
Content-Type: application/json

{
  "name": "NewDB",
  "connectionString": "Server=localhost;Database=NewDB;User Id=user;Password=pass;",
  "databaseType": 1,
  "isEnabled": true,
  "description": "新数据库"
}
```

### 6. 更新连接配置
```http
PUT /api/Database/config
Content-Type: application/json

{
  "id": 1,
  "name": "UpdatedDB",
  "connectionString": "Server=localhost;Database=UpdatedDB;User Id=user;Password=pass;",
  "databaseType": 1,
  "isEnabled": true,
  "description": "更新的数据库"
}
```

### 7. 删除连接配置
```http
DELETE /api/Database/config/1
```

### 8. 启用/禁用连接配置
```http
POST /api/Database/config/1/toggle?isEnabled=true
```

## 💻 使用示例

### C# 服务中使用

```csharp
public class ExternalDataService
{
    private readonly IDynamicDatabaseService _databaseService;
    private readonly ILogger<ExternalDataService> _logger;

    public ExternalDataService(IDynamicDatabaseService databaseService, ILogger<ExternalDataService> logger)
    {
        _databaseService = databaseService;
        _logger = logger;
    }

    public async Task<List<ExternalData>> GetExternalDataAsync()
    {
        try
        {
            // 获取外部数据库连接
            var connection = _databaseService.GetConnection("ExternalDB1");
            
            // 执行查询
            var data = await connection.Queryable<ExternalData>()
                .Where(x => x.IsActive)
                .ToListAsync();

            return data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取外部数据失败");
            return new List<ExternalData>();
        }
    }

    public async Task<bool> TestExternalConnectionAsync()
    {
        try
        {
            return await _databaseService.TestConnectionAsync("ExternalDB1");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "测试外部连接失败");
            return false;
        }
    }
}
```

### 控制器中使用

```csharp
[ApiController]
[Route("api/[controller]")]
public class ExternalDataController : ControllerBase
{
    private readonly IDynamicDatabaseService _databaseService;

    public ExternalDataController(IDynamicDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [HttpGet("data")]
    public async Task<IActionResult> GetData([FromQuery] string connectionName)
    {
        try
        {
            var connection = _databaseService.GetConnection(connectionName);
            var data = await connection.Queryable<ExternalData>().ToListAsync();
            return Ok(data);
        }
        catch (Exception ex)
        {
            return BadRequest($"获取数据失败: {ex.Message}");
        }
    }
}
```

## 🔧 高级配置

### 1. 连接池配置

```csharp
// 在DynamicDatabaseService中配置连接池
var connectionConfig = new ConnectionConfig
{
    ConnectionString = config.ConnectionString,
    DbType = config.DatabaseType,
    IsAutoCloseConnection = true,
    InitKeyType = InitKeyType.Attribute,
    // 连接池配置
    ConnectionPoolSize = 10,
    CommandTimeOut = 30
};
```

### 2. 日志配置

```csharp
// 配置SQL日志
client.Aop.OnLogExecuting = (sql, pars) =>
{
    _logger.LogDebug($"执行SQL: {sql}");
    if (pars != null && pars.Length > 0)
    {
        _logger.LogDebug($"参数: {string.Join(", ", pars.Select(p => $"{p.ParameterName}={p.Value}"))}");
    }
};
```

### 3. 错误处理

```csharp
client.Aop.OnError = (exp) =>
{
    _logger.LogError(exp, $"数据库执行错误: {config.Name}");
    // 可以在这里添加重试逻辑
};
```

## 📊 性能优化

### 1. 连接池管理
- 自动管理连接池大小
- 避免重复创建连接
- 支持连接复用

### 2. 缓存机制
- 连接配置缓存
- 连接实例缓存
- 减少数据库查询

### 3. 异步操作
- 支持异步数据库操作
- 非阻塞连接测试
- 并发安全

## 🔍 故障排除

### 常见问题

#### 1. 连接字符串错误
```
错误: 无法连接到数据库
解决: 检查连接字符串格式和数据库服务状态
```

#### 2. 数据库类型不匹配
```
错误: 不支持的数据库类型
解决: 检查DatabaseType配置是否正确
```

#### 3. 连接池耗尽
```
错误: 连接池已满
解决: 增加连接池大小或检查连接是否正确释放
```

#### 4. 权限不足
```
错误: 访问被拒绝
解决: 检查数据库用户权限
```

## 📈 监控和日志

### 1. 连接状态监控
```csharp
// 监控连接状态
var connections = _databaseService.GetAvailableConnections();
foreach (var connectionName in connections)
{
    var isHealthy = await _databaseService.TestConnectionAsync(connectionName);
    _logger.LogInformation($"连接 {connectionName} 状态: {(isHealthy ? "正常" : "异常")}");
}
```

### 2. 性能监控
```csharp
// 监控连接性能
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
var connection = _databaseService.GetConnection("ExternalDB1");
stopwatch.Stop();
_logger.LogInformation($"获取连接耗时: {stopwatch.ElapsedMilliseconds}ms");
```

## 🔄 更新日志

- v1.0.0 - 基础动态连接管理功能
- v1.1.0 - 添加连接池管理
- v1.2.0 - 支持配置文件方式
- v1.3.0 - 添加连接测试和监控
- v1.4.0 - 优化性能和错误处理
