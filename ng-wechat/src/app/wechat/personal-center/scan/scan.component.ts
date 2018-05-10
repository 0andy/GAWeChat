import { Component, ViewEncapsulation, Injector, OnInit } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/timer';
import { AppComponentBase } from '../../components/app-component-base';
import { Router } from '@angular/router';
import { WechatUser, Shop, UserType, ShopGoods } from '../../../services/model';
import { ShopService, AppConsts, WechatUserService } from '../../../services';
import { JWeiXinService } from 'ngx-weui/jweixin';
import { ToptipsService } from "ngx-weui/toptips";

@Component({
    selector: 'wechat-scan',
    templateUrl: './scan.component.html',
    styleUrls: ['./scan.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class ScanComponent extends AppComponentBase implements OnInit {

    cardNum: string;
    goodsBarCode: string;
    num: number = 1;
    num1: number = 1;

    user: WechatUser;
    shop: Shop;
    member: WechatUser;//会员
    goods = [];

    constructor(injector: Injector,
        private shopService: ShopService,
        private wechatUserService: WechatUserService,
        private router: Router,
        private wxService: JWeiXinService,
        private srv: ToptipsService) {
        super(injector);
    }

    ngOnInit() {
        this.wxService.get().then((res) => {
            if (!res) {
                console.warn('jweixin.js 加载失败');
                return;
            }
            //alert(location.href.split('#')[0])
            //alert(encodeURIComponent(location.href.split('#')[0]));
            let url = encodeURIComponent(location.href.split('#')[0]);
            this.settingsService.getJsApiConfig(url).subscribe(result => {
                if (result) {
                    result.jsApiList = ['scanQRCode'];//指定调用的接口名
                    //console.log(result.toJSON());
                    // 1、通过config接口注入权限验证配置
                    wx.config(result.toJSON());
                    // 2、通过ready接口处理成功验证
                    wx.ready(() => {
                        // 注册各种onMenuShareTimeline & onMenuShareAppMessage
                    });
                    // 2、通过error接口处理失败验证
                    wx.error(() => {

                    });
                }
            });
        });

        this.settingsService.getUser().subscribe(result => {
            this.user = result;
            if (this.user) {
                //console.table(this.user);
                if (this.user.userType != UserType.Retailer) { //不是零售客户需先绑定
                    this.router.navigate(['/personals/bind-retailer']);
                } else {
                    if (!this.user.isShopkeeper && this.user.status == 0) {//不是店主 且 未审核
                        this.router.navigate(['/shops/wait-audit']);
                    } else {
                        this.shopService.GetShopByOpenId(this.WUserParams)
                            .subscribe(result => {
                                this.shop = result;
                                if (!this.shop) {//如果没有店铺 需要新增
                                    this.router.navigate(['/shops/shop-add']);
                                }
                            });
                    }
                }
            } else {
                this.router.navigate(['/personals/bind-retailer']);
            }
        });
    }

    setCardNum(res: string) {
        let resarry = res.split(',');
        if (resarry.length == 2) {
            if (resarry[0] != 'CODE_128') {
                this.srv['warn']('条码格式不匹配');
                return;
            }
            this.cardNum = resarry[1];
            //获取会员数据
            this.wechatUserService.GetWeChatUserByMemberBarCodeAsync(this.cardNum, this.settingsService.tenantId).subscribe(result => {
                this.member = result;
                this.srv['success']('扫码成功');
            });
        } else {
            this.srv['warn']('条码格式不匹配');
        }
    }

    //调用微信扫一扫
    wxScanQRCode(): Promise<string>{
        return new Promise<string>((resolve, reject) => {
            wx.scanQRCode({
                needResult: 1, // 默认为0，扫描结果由微信处理，1则直接返回扫描结果，
                //scanType: ["qrCode", "barCode"], // 可以指定扫二维码还是一维码，默认二者都有
                scanType: ['barCode'],
                success: ((res) => {
                    resolve(res.resultStr);
                })
            });
            //resolve('EAN_13,6901028042758');
        });
    }

    scanCard() {
        this.wxScanQRCode().then((res) =>{
            this.setCardNum(res);
        });
    }

    findGoods() {
        let bo = false;
        this.goods.forEach((item) => {
            if (item.packageCode == this.goodsBarCode) {//包码
                item.num = item.num + 1;
                bo = true;
            } else if (item.barCode == this.goodsBarCode) {//条码
                item.num = item.num + 10;
                bo = true;
            }
        });
        return bo;
    }

    setGoodsBarCode(res: string) {
        let resarry = res.split(',');
        if (resarry.length == 2) {
            if (resarry[0] != 'EAN_13') {
                this.srv['warn']('条码格式不匹配');
                return;
            }
            this.goodsBarCode = resarry[1];
            if (!this.findGoods()) {
                //获取卷烟数据
                let param: any = {};
                param.code = this.goodsBarCode;
                if (this.settingsService.tenantId) {
                    param.tenantId = this.settingsService.tenantId;
                }
                this.shopService.GetShopProductByCode(param).subscribe(result => {
                    if (result) {
                        this.goods.push(result);
                        this.srv['success']('扫码成功');
                    } else {
                        this.srv['warn']('没找到匹配商品');
                    }
                });
            }
        } else {
            this.srv['warn']('条码格式不匹配');
        }
    }

    scanGoodsBarCode() {
        this.wxScanQRCode().then((res) =>{
            this.setGoodsBarCode(res);
        });
    }

    onSave() {

    }
}
