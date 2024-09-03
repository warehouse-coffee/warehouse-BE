//----------------------
// <auto-generated>
//     Generated using the NSwag toolchain v14.0.3.0 (NJsonSchema v11.0.0.0 (Newtonsoft.Json v13.0.0.0)) (http://NSwag.org)
// </auto-generated>
//----------------------

/* tslint:disable */
/* eslint-disable */
// ReSharper disable InconsistentNaming

import followIfLoginRedirect from './api-authorization/followIfLoginRedirect';

export class CompaniesClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    createCompany(command: CreateCompanyCommand): Promise<ResponseDto> {
        let url_ = this.baseUrl + "/api/Companies";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(command);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processCreateCompany(_response);
        });
    }

    protected processCreateCompany(response: Response): Promise<ResponseDto> {
        followIfLoginRedirect(response);
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = ResponseDto.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ResponseDto>(null as any);
    }

    getCompanyList(): Promise<ResponseDto> {
        let url_ = this.baseUrl + "/api/Companies";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetCompanyList(_response);
        });
    }

    protected processGetCompanyList(response: Response): Promise<ResponseDto> {
        followIfLoginRedirect(response);
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = ResponseDto.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ResponseDto>(null as any);
    }
}

export class IdentityUserClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    userRegister(command: CreateUserCommand): Promise<ResponseDto> {
        let url_ = this.baseUrl + "/api/IdentityUser/userregister";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(command);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processUserRegister(_response);
        });
    }

    protected processUserRegister(response: Response): Promise<ResponseDto> {
        followIfLoginRedirect(response);
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = ResponseDto.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ResponseDto>(null as any);
    }

    signIn(query: SignInCommand): Promise<SignInVm> {
        let url_ = this.baseUrl + "/api/IdentityUser/signin";
        url_ = url_.replace(/[?&]$/, "");

        const content_ = JSON.stringify(query);

        let options_: RequestInit = {
            body: content_,
            method: "POST",
            headers: {
                "Content-Type": "application/json",
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processSignIn(_response);
        });
    }

    protected processSignIn(response: Response): Promise<SignInVm> {
        followIfLoginRedirect(response);
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = SignInVm.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<SignInVm>(null as any);
    }
}

export class ProductsClient {
    private http: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> };
    private baseUrl: string;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined = undefined;

    constructor(baseUrl?: string, http?: { fetch(url: RequestInfo, init?: RequestInit): Promise<Response> }) {
        this.http = http ? http : window as any;
        this.baseUrl = baseUrl !== undefined && baseUrl !== null ? baseUrl : "";
    }

    getProductList(): Promise<ProductListVM> {
        let url_ = this.baseUrl + "/api/Products";
        url_ = url_.replace(/[?&]$/, "");

        let options_: RequestInit = {
            method: "GET",
            headers: {
                "Accept": "application/json"
            }
        };

        return this.http.fetch(url_, options_).then((_response: Response) => {
            return this.processGetProductList(_response);
        });
    }

    protected processGetProductList(response: Response): Promise<ProductListVM> {
        followIfLoginRedirect(response);
        const status = response.status;
        let _headers: any = {}; if (response.headers && response.headers.forEach) { response.headers.forEach((v: any, k: any) => _headers[k] = v); };
        if (status === 200) {
            return response.text().then((_responseText) => {
            let result200: any = null;
            let resultData200 = _responseText === "" ? null : JSON.parse(_responseText, this.jsonParseReviver);
            result200 = ProductListVM.fromJS(resultData200);
            return result200;
            });
        } else if (status !== 200 && status !== 204) {
            return response.text().then((_responseText) => {
            return throwException("An unexpected server error occurred.", status, _responseText, _headers);
            });
        }
        return Promise.resolve<ProductListVM>(null as any);
    }
}

export class ResponseDto implements IResponseDto {
    statusCode?: number;
    message?: string;
    data?: any | undefined;

    constructor(data?: IResponseDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.statusCode = _data["statusCode"];
            this.message = _data["message"];
            this.data = _data["data"];
        }
    }

    static fromJS(data: any): ResponseDto {
        data = typeof data === 'object' ? data : {};
        let result = new ResponseDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["statusCode"] = this.statusCode;
        data["message"] = this.message;
        data["data"] = this.data;
        return data;
    }
}

export interface IResponseDto {
    statusCode?: number;
    message?: string;
    data?: any | undefined;
}

export class CreateCompanyCommand implements ICreateCompanyCommand {
    companyId?: string | undefined;
    companyName?: string | undefined;
    phoneContact?: string | undefined;

    constructor(data?: ICreateCompanyCommand) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.companyId = _data["companyId"];
            this.companyName = _data["companyName"];
            this.phoneContact = _data["phoneContact"];
        }
    }

    static fromJS(data: any): CreateCompanyCommand {
        data = typeof data === 'object' ? data : {};
        let result = new CreateCompanyCommand();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["companyId"] = this.companyId;
        data["companyName"] = this.companyName;
        data["phoneContact"] = this.phoneContact;
        return data;
    }
}

export interface ICreateCompanyCommand {
    companyId?: string | undefined;
    companyName?: string | undefined;
    phoneContact?: string | undefined;
}

export class CreateUserCommand implements ICreateUserCommand {
    userRegister?: UserRegister;

    constructor(data?: ICreateUserCommand) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.userRegister = _data["userRegister"] ? UserRegister.fromJS(_data["userRegister"]) : <any>undefined;
        }
    }

    static fromJS(data: any): CreateUserCommand {
        data = typeof data === 'object' ? data : {};
        let result = new CreateUserCommand();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["userRegister"] = this.userRegister ? this.userRegister.toJSON() : <any>undefined;
        return data;
    }
}

export interface ICreateUserCommand {
    userRegister?: UserRegister;
}

export class UserRegister implements IUserRegister {
    userName?: string | undefined;
    password?: string | undefined;
    email?: string | undefined;
    phoneNumber?: string | undefined;
    companyId?: string | undefined;
    roleName?: string | undefined;

    constructor(data?: IUserRegister) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.userName = _data["userName"];
            this.password = _data["password"];
            this.email = _data["email"];
            this.phoneNumber = _data["phoneNumber"];
            this.companyId = _data["companyId"];
            this.roleName = _data["roleName"];
        }
    }

    static fromJS(data: any): UserRegister {
        data = typeof data === 'object' ? data : {};
        let result = new UserRegister();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["userName"] = this.userName;
        data["password"] = this.password;
        data["email"] = this.email;
        data["phoneNumber"] = this.phoneNumber;
        data["companyId"] = this.companyId;
        data["roleName"] = this.roleName;
        return data;
    }
}

export interface IUserRegister {
    userName?: string | undefined;
    password?: string | undefined;
    email?: string | undefined;
    phoneNumber?: string | undefined;
    companyId?: string | undefined;
    roleName?: string | undefined;
}

export class SignInVm implements ISignInVm {
    token?: string | undefined;
    statusCode?: number;

    constructor(data?: ISignInVm) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.token = _data["token"];
            this.statusCode = _data["statusCode"];
        }
    }

    static fromJS(data: any): SignInVm {
        data = typeof data === 'object' ? data : {};
        let result = new SignInVm();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["token"] = this.token;
        data["statusCode"] = this.statusCode;
        return data;
    }
}

export interface ISignInVm {
    token?: string | undefined;
    statusCode?: number;
}

export class SignInCommand implements ISignInCommand {
    email?: string | undefined;
    password?: string | undefined;

    constructor(data?: ISignInCommand) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            this.email = _data["email"];
            this.password = _data["password"];
        }
    }

    static fromJS(data: any): SignInCommand {
        data = typeof data === 'object' ? data : {};
        let result = new SignInCommand();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        data["email"] = this.email;
        data["password"] = this.password;
        return data;
    }
}

export interface ISignInCommand {
    email?: string | undefined;
    password?: string | undefined;
}

export class ProductListVM implements IProductListVM {
    productList?: ProductDto[] | undefined;

    constructor(data?: IProductListVM) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
        if (_data) {
            if (Array.isArray(_data["productList"])) {
                this.productList = [] as any;
                for (let item of _data["productList"])
                    this.productList!.push(ProductDto.fromJS(item));
            }
        }
    }

    static fromJS(data: any): ProductListVM {
        data = typeof data === 'object' ? data : {};
        let result = new ProductListVM();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        if (Array.isArray(this.productList)) {
            data["productList"] = [];
            for (let item of this.productList)
                data["productList"].push(item.toJSON());
        }
        return data;
    }
}

export interface IProductListVM {
    productList?: ProductDto[] | undefined;
}

export class ProductDto implements IProductDto {

    constructor(data?: IProductDto) {
        if (data) {
            for (var property in data) {
                if (data.hasOwnProperty(property))
                    (<any>this)[property] = (<any>data)[property];
            }
        }
    }

    init(_data?: any) {
    }

    static fromJS(data: any): ProductDto {
        data = typeof data === 'object' ? data : {};
        let result = new ProductDto();
        result.init(data);
        return result;
    }

    toJSON(data?: any) {
        data = typeof data === 'object' ? data : {};
        return data;
    }
}

export interface IProductDto {
}

export class SwaggerException extends Error {
    override message: string;
    status: number;
    response: string;
    headers: { [key: string]: any; };
    result: any;

    constructor(message: string, status: number, response: string, headers: { [key: string]: any; }, result: any) {
        super();

        this.message = message;
        this.status = status;
        this.response = response;
        this.headers = headers;
        this.result = result;
    }

    protected isSwaggerException = true;

    static isSwaggerException(obj: any): obj is SwaggerException {
        return obj.isSwaggerException === true;
    }
}

function throwException(message: string, status: number, response: string, headers: { [key: string]: any; }, result?: any): any {
    if (result !== null && result !== undefined)
        throw result;
    else
        throw new SwaggerException(message, status, response, headers, null);
}