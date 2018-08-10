using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ohayoo.Api
{
    public enum HttpStatusCodeDefine
    {
        UNKNOWN_EXCEPTION = 1,
        SOCKET_NOT_CONNECTED = 2,
        NOT_FOUND=400,
        SERVER_ERROR=500,
        SUCCESS=200,
        UNAUTHORIZED_ERROR=401,
        API_INVALID = 100,
        PARAMS_INVALID = 101,
        DATA_INVALID = 102,
        SESSION_TIMEOUT = 103,
        REQUEST_TIMEOUT = 104,
        EMAIL_INVALID=105,
        EMAIL_EXIST = 106,
        USERNAME_INVALID=107,
        USERNAME_EXIST=108,
        NAME_EXIST=109,
        PHONE_EXIST=110,
        ZIPCODE_ERROR=111,
        LOGIN_INVALID=112,
        NO_DATA=113,
        VERIFY_CODE=114,
        REQUIRED_INVALID=115,
        ZIPIDYNUMBER_INVALID=116,
        ZIPCODE_NOT_EXIST=117,
        ERROR_SIGNUP=118,
        UPDATE_PROFILE_FAIL=119,
        CODE_INVALID=120,
        RESEND_ERROR=121,
        UPLOAD_INVALID=122,
        DIVIDEBYZERO_EXCEPTION=123
    }
}